using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building.Data
{
	[CreateAssetMenu(fileName = "BuildDatabase", menuName = "Building/BuildDatabase")]
	public class BuildDatabase : ScriptableObject
	{
		[SerializeField] private List<Tile> m_tiles;

		public List<Tile> Tiles => m_tiles;
	}
}
