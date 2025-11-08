using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building
{
	public class TilePlacer : MonoBehaviour
	{
		[SerializeField] private Tilemap m_tilemap;
		[SerializeField] private Tile m_tileToPlace;

		public static TilePlacer Instance => s_instance;
		private static TilePlacer s_instance; // I hate this..

		private void Start ()
		{
			s_instance = this;
		}

		public void Stop ()
		{
			m_tilemap = null;
			m_tileToPlace = null;
		}

		public void SetTileToPlace (Tile tile)
		{
			m_tileToPlace = tile;
		}

		public void SetTileMap (Tilemap map)
		{
			m_tilemap = map;
		}

		void Update ()
		{
			if(m_tilemap == null || m_tileToPlace == null) return;

			if (Input.GetMouseButton(0)) // left-click
			{
				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mouseWorldPos.z = 0;
				Vector3Int cellPos = m_tilemap.WorldToCell(mouseWorldPos);
				cellPos.z = 0;

				m_tilemap.SetTile(cellPos, m_tileToPlace);
			}
		}
	}
}