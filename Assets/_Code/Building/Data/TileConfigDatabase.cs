using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building.Data
{
	[CreateAssetMenu(fileName = "TileConfigDatabase", menuName = "Databases/TileConfigDatabase")]
	public class TileConfigDatabase : ScriptableObject
	{
		[SerializeField] private List<TileConfig> m_allTileConfigs = new List<TileConfig>();

		public TileType GetType(Tile tile)
		{
			var t = m_allTileConfigs.FirstOrDefault(c => c.Tile == tile);

			if (t == null) return TileType.None;

			return t.TileType;
		}

		public string GetTileID (Tile tile)
		{
			var ti = m_allTileConfigs.FirstOrDefault(t => t.Tile == tile);
			return ti != null ? ti.name : "";
		}

		public TileBase GetTileByID (string tileID)
		{
			var ti = m_allTileConfigs.FirstOrDefault(t => t.name == tileID);
			return ti != null ? ti.Tile : null;
		}

		public IReadOnlyList<TileConfig> AllTileConfigs => m_allTileConfigs;

#if UNITY_EDITOR
		[ContextMenu("Refresh Database")]
		public void Refresh ()
		{
			string[] guids = AssetDatabase.FindAssets("t:TileConfig");
			m_allTileConfigs = guids
				.Select(guid => AssetDatabase.LoadAssetAtPath<TileConfig>(AssetDatabase.GUIDToAssetPath(guid)))
				.Where(t => t != null)
				.ToList();

			EditorUtility.SetDirty(this);
			Debug.Log($"TileConfigDatabase refreshed. Found {m_allTileConfigs.Count} TileConfigs.");
		}
#endif
	}
}
