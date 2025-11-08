using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Building
{
	public class TilePlacementHover : MonoBehaviour
	{
		[SerializeField] private Color m_validColor = Color.green;
		[SerializeField] private Color m_inValidColor = Color.red;

		[SerializeField] private SpriteRenderer m_renderer;

		[SerializeField] private Tilemap m_groundTileMap;

		private bool m_canBePlaced;

		public bool CanBePlaced => m_canBePlaced;

		private void Start ()
		{
			Hide();
		}

		public void SetPosition (Vector3Int position, Tile tile, Tilemap tilemap)
		{
			gameObject.SetActive(true);

			transform.position = tilemap.CellToWorld(position + new Vector3Int(0,0,1));

			if(m_groundTileMap.HasTile(position + Vector3Int.right) ||
				m_groundTileMap.HasTile(position + Vector3Int.left) ||
				m_groundTileMap.HasTile(position + Vector3Int.up) ||
				m_groundTileMap.HasTile(position + Vector3Int.down))
			{
				m_canBePlaced = true;
			} else { m_canBePlaced = false; }


			m_renderer.sprite = tile.sprite;
			m_renderer.color = m_canBePlaced ? m_validColor : m_inValidColor;
		}
		
		public void Hide ()
		{
			gameObject.SetActive(false);
		}
	}
}
