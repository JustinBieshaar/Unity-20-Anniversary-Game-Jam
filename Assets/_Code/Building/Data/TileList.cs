using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building.Data
{

	[CreateAssetMenu(fileName = "TileList", menuName = "Databases/TileList")]
	public class TileList : ScriptableObject
	{
		[SerializeField] private List<Tile> m_tiles;

		public List<Tile> Tiles => m_tiles;
		public int Count => m_tiles.Count;

		public bool Contains(Tile tile)
		{
			return m_tiles.Contains(tile);
		}
	}
}
