using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building.Data
{
	[CreateAssetMenu(fileName = "TileConfig", menuName = "Databases/TileConfig")]
	public class TileConfig : ScriptableObject
	{
		[SerializeField] private TileType m_type;
		[SerializeField] private Tile m_tile;
		[SerializeField] private TileList m_acceptableTiles;
		[SerializeField] private bool m_blockByEnvironment;

		public TileType TileType => m_type;
		public Tile Tile => m_tile;
		public TileList AcceptableTiles => m_acceptableTiles;
		public bool BlockByEnvironment => m_blockByEnvironment;

		public TileConfig(Tile tile) { m_tile = tile; }

		public bool CanPlace (Tilemap ground, Tilemap environment, Vector3Int pos)
		{
			if (m_blockByEnvironment)
			{
				if (environment.HasTile(pos))
				{
					return false;
				}
			}
			if (m_acceptableTiles == null || m_acceptableTiles.Count == 0)
				return true;

			return m_acceptableTiles.Contains(ground.GetTile<Tile>(pos));
		}
	}
}
