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
			public int TreeCount;
		}

		[SerializeField] private TileConfigDatabase m_tileDatabase;
		[SerializeField] private BlobDatabase m_blobDatabase;

		[SerializeField] private Tilemap m_ground;
		[SerializeField] private Tilemap m_environment;

		private static BlobManager s_instance;
		public static BlobManager Instance => s_instance;

		private List<Blob> m_blobs = new List<Blob>();

		private Dictionary<Vector3Int, bool> m_visited = new();

		private readonly Vector3Int[] NeighborDirs =
		{
			Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
		};

		private void Start ()
		{
			s_instance = this;
			Distribute();
		}

		public void Distribute ()
		{
			ClearBlobs();
			var result = FindRegions();

			foreach (var region in result)
			{
				BlobRequirements blob = m_blobDatabase.GetBlobByTileType(region.Type);
				if(blob == null)
				{
					Debug.Log($"No blob for {region.Type}");
					continue;
				}
				int blobCount = CalculateBlobCount(blob, region.Tiles.Count, region.TreeCount);

				Debug.Log($"Adding blobs: {blobCount} to region: {region.Type}");

				for (int i = 0; i < blobCount; i++) 
				{
					var randomTile = region.Tiles[Random.Range(0, region.Tiles.Count)];
					var blobInstance = Instantiate(blob.BlobPrefab);
					blobInstance.transform.position = m_ground.CellToWorld(randomTile);

					m_blobs.Add(blobInstance);
				}
			}
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

	}
}
