using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building.Data
{
	[CreateAssetMenu(fileName = "BuildDatabase", menuName = "Building/BuildDatabase")]
	public class BuildDatabase : ScriptableObject
	{
		[Serializable]
		public class TileRule
		{
			public Tile Tile;
			public List<Tile> AcceptableTiles;

			public bool CanPlace(Tilemap ground, Vector3Int pos)
			{
				if (AcceptableTiles == null || AcceptableTiles.Count == 0)
					return true;

				return AcceptableTiles.Contains(ground.GetTile<Tile>(pos));
			}
		}
		[SerializeField] private List<TileRule> m_tiles;

		public List<TileRule> Tiles => m_tiles;
	}
}
