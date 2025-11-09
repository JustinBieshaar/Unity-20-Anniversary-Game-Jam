using Assets._Code.Blobs.Data;
using Assets._Code.Building.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Blobs
{
	public class BlobManager : MonoBehaviour
	{
		public class GroundRegion
		{
			public TileType Type;
			public List<Vector3Int> Tiles = new();
			public HashSet<Vector3Int> TileSet = new(); // for fast lookup
			public Dictionary<Vector3Int, Vector3> WorldPositions = new(); // precomputed cell centers
			public Dictionary<Vector3Int, List<Vector3Int>> Neighbors = new(); // adjacency list
			public int TreeCount;
			public int ID;

			public Vector3 GetWorld (Vector3Int cell)
			{
				if (WorldPositions.TryGetValue(cell, out var w))
					return w;
				return Vector3.zero;
			}
		}

		[SerializeField] private TileConfigDatabase m_tileDatabase;
		[SerializeField] private BlobDatabase m_blobDatabase;

		[SerializeField] private Tilemap m_ground;
		[SerializeField] private Tilemap m_environment;

		private static BlobManager s_instance;
		public static BlobManager Instance => s_instance;

		private List<Blob> m_blobs = new List<Blob>();
		private Dictionary<string, List<Blob>> m_regionBlobs = new();

		private Dictionary<Vector3Int, bool> m_visited = new();

		private Dictionary<string, HashSet<Vector3Int>> m_regionTiles = new();
		private int m_nextRegionId = 1;


		private readonly Vector3Int[] NeighborDirs =
		{
			Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
		};

		private void Awake ()
		{
			s_instance = this;
		}

		private void Start ()
		{
			Distribute();
		}

		public void Distribute ()
		{
			m_visited?.Clear();
			var newRegions = FindRegions();
			var newKeys = new HashSet<string>();
			var nextCache = new Dictionary<string, List<Blob>>();
			var nextTiles = new Dictionary<string, HashSet<Vector3Int>>();

			foreach (var region in newRegions)
			{
				// --- Try to find an existing cached region with same type and overlapping tiles ---
				string matchedKey = null;
				foreach (var kv in m_regionTiles)
				{
					string oldKey = kv.Key;
					var oldTiles = kv.Value;
					if (!m_regionBlobs.ContainsKey(oldKey)) continue;

					// type match?
					if (m_blobDatabase.GetBlobByTileType(region.Type) == null) continue;

					// overlap?
					bool overlaps = false;
					foreach (var t in region.Tiles)
					{
						if (oldTiles.Contains(t)) { overlaps = true; break; }
					}

					if (overlaps)
					{
						matchedKey = oldKey;
						break;
					}
				}

				string key;
				if (matchedKey != null)
				{
					// reuse cached id
					key = matchedKey;
				}
				else
				{
					// brand new region
					key = $"R_{m_nextRegionId++}_{region.Type}";
				}

				newKeys.Add(key);
				nextTiles[key] = new HashSet<Vector3Int>(region.Tiles);

				BlobRequirements blobReq = m_blobDatabase.GetBlobByTileType(region.Type);
				if (blobReq == null) continue;

				int desiredCount = CalculateBlobCount(blobReq, region.Tiles.Count, region.TreeCount);

				if (!m_regionBlobs.TryGetValue(key, out var blobList))
					blobList = new List<Blob>();

				// remove excess
				while (blobList.Count > desiredCount)
				{
					var last = blobList[^1];
					blobList.RemoveAt(blobList.Count - 1);
					m_blobs.Remove(last);
					Destroy(last.gameObject);
				}

				// add missing
				while (blobList.Count < desiredCount)
				{
					var randomTile = region.Tiles[Random.Range(0, region.Tiles.Count)];
					var blobInstance = Instantiate(blobReq.BlobPrefab);
					blobInstance.transform.position = region.WorldPositions[randomTile] + new Vector3(0, 0.25f, 0);
					blobInstance.Init(blobReq, region);

					blobList.Add(blobInstance);
					m_blobs.Add(blobInstance);
				}

				// optional: refresh region on existing blobs
				foreach (var b in blobList)
					b.SetRegion(region);

				nextCache[key] = blobList;
			}

			// remove any old cached regions not in new set
			foreach (var kv in m_regionBlobs)
			{
				if (!newKeys.Contains(kv.Key))
				{
					foreach (var b in kv.Value)
						Destroy(b.gameObject);
				}
			}

			// replace caches
			m_regionBlobs = nextCache;
			m_regionTiles = nextTiles;
		}

		private void ClearBlobs ()
		{
			for (int i = m_blobs.Count - 1; i >= 0; i--)
			{
				Destroy(m_blobs[i].gameObject);
			}
			m_blobs.Clear();
		}

		private int CalculateBlobCount (BlobRequirements blob, int tileCount, int treeCount)
		{
			int minTiles = blob.TileRequirement;
			int tilesPerBlob = blob.TileRequirementPerBlob;
			int minTrees = blob.MinTreeRequirement;
			int treeSaturation = blob.TreeSaturation;

			// Must meet basic requirements
			if (tileCount < minTiles || treeCount < minTrees)
				return 0;

			// Compute base spawn potential based on area
			float areaFactor = (tileCount - minTiles) / tilesPerBlob;

			// Compute tree factor: helps early, suppresses late
			float treeFactor = 1.0f;
			if (treeSaturation > 0)
			{
				treeFactor = (float)treeSaturation / (treeSaturation + (float)treeCount);
			}

			// Combine the two
			float blobScore = areaFactor * treeFactor;

			int blobCount = Mathf.FloorToInt(blobScore);

			// Clamp so you don’t get absurd numbers
			return Mathf.Clamp(blobCount, 0, 10);
		}

		private List<GroundRegion> FindRegions ()
		{
			List<GroundRegion> result = new();

			BoundsInt bounds = m_ground.cellBounds;
			for (int x = bounds.xMin; x < bounds.xMax; x++)
			{
				for (int y = bounds.yMin; y < bounds.yMax; y++)
				{
					Vector3Int cell = new Vector3Int(x, y, 0);
					if (m_visited.ContainsKey(cell)) continue;

					Tile tile = m_ground.GetTile<Tile>(cell);
					if (tile == null) continue;

					TileType type = m_tileDatabase.GetType(tile); // You’ll need to implement this helper
					if (type == TileType.None) continue;

					// Flood fill to find connected tiles of the same type
					GroundRegion region = new GroundRegion { Type = type };
					FloodFill(cell, type, region);
					FinalizeRegion(region);
					CountEnvironment(region);
					result.Add(region);
				}
			}
			return result;
		}

		private void FloodFill (Vector3Int start, TileType type, GroundRegion region)
		{
			Queue<Vector3Int> queue = new();
			queue.Enqueue(start);
			m_visited[start] = true;

			while (queue.Count > 0)
			{
				var cell = queue.Dequeue();
				region.Tiles.Add(cell);

				foreach (var dir in NeighborDirs)
				{
					Vector3Int neighbor = cell + dir;
					if (m_visited.ContainsKey(neighbor)) continue;

					Tile tile = m_ground.GetTile<Tile>(neighbor);
					if (tile == null) continue;

					TileType neighborType = m_tileDatabase.GetType(tile);
					if (neighborType != type) continue;
					if (neighborType == TileType.None) continue;

					m_visited[neighbor] = true;
					queue.Enqueue(neighbor);
				}
			}
		}

		private void FinalizeRegion (GroundRegion region)
		{
			foreach (var cell in region.Tiles)
			{
				region.TileSet.Add(cell);
				region.WorldPositions[cell] = m_ground.GetCellCenterWorld(cell);
			}

			// Build neighbor graph
			foreach (var cell in region.Tiles)
			{
				List<Vector3Int> neighbors = new();
				foreach (var dir in NeighborDirs)
				{
					var n = cell + dir;
					if (region.TileSet.Contains(n))
						neighbors.Add(n);
				}
				region.Neighbors[cell] = neighbors;
			}
		}

		private void CountEnvironment (GroundRegion region)
		{
			int trees = 0;
			foreach (var cell in region.Tiles)
			{
				var envTile = m_environment.GetTile<Tile>(cell);
				if (envTile == null) continue;

				TileType envType = m_tileDatabase.GetType(envTile);
				if (envType == TileType.Tree)
					trees++;
			}
			region.TreeCount = trees;
		}

		private string ComputeRegionKey (GroundRegion region)
		{
			// 1. Check cached regions for overlap
			foreach (var kv in m_regionTiles)
			{
				string cachedKey = kv.Key;
				var cachedTiles = kv.Value;

				// only consider regions of same type
				if (!m_regionBlobs.ContainsKey(cachedKey)) continue;
				if (m_blobDatabase.GetBlobByTileType(region.Type) == null) continue;

				// check if any tile overlaps
				bool overlaps = false;
				foreach (var t in region.Tiles)
				{
					if (cachedTiles.Contains(t)) { overlaps = true; break; }
				}

				if (overlaps)
					return cachedKey; // reuse existing key
			}

			// 2. No overlapping cached region: generate a new hash-based key
			var sorted = new List<Vector3Int>(region.Tiles);
			sorted.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

			var hash = 17;
			foreach (var cell in sorted)
				hash = hash * 31 + cell.GetHashCode();

			return $"{region.Type}_{hash}";
		}

	}
}
