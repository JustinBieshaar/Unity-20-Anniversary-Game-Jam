using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building
{
	public class TilePlacer : MonoBehaviour
	{
		[SerializeField] private Tilemap m_tilemap;
		[SerializeField] private TileBase m_tileToPlace;

		void Update ()
		{
			if (Input.GetMouseButtonDown(0)) // left-click
			{
				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int cellPos = m_tilemap.WorldToCell(mouseWorldPos);
				cellPos.z = 1;

				m_tilemap.SetTile(cellPos, m_tileToPlace);
				Debug.Log($"Place tile! ${cellPos}");
			}
		}
	}
}