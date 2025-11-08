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

		[SerializeField] private Tilemap m_ground;
		[SerializeField] private Tilemap m_environment;

		private Dictionary<Vector3Int, bool> m_visited = new();

		private readonly Vector3Int[] NeighborDirs =
		{
			Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
		};

		private void Start ()
		{
			var result = FindRegions();
			Debug.Log($"Regions found: {result.Count}");

			foreach (var item in result)
			{
				Debug.Log($"Region: {item.Type}, count: {item.Tiles.Count}");
			}
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
					Debug.Log($"neighbour: {neighbor.x}, {neighbor.y} is of type: {neighborType} and tile: {tile.sprite}");
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
