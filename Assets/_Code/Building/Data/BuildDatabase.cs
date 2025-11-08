using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building.Data
{
	[CreateAssetMenu(fileName = "BuildDatabase", menuName = "Databases/BuildDatabase")]
	public class BuildDatabase : ScriptableObject
	{
		[SerializeField] private List<TileConfig> m_tiles;

		public List<TileConfig> Tiles => m_tiles;

		public TileConfig FindConfig (Tile tile)
		{
			return m_tiles.First(t => t.Tile  == tile);
		}
	}
}
