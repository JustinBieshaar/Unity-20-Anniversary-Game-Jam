using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Assets._Code.Building.Data.BuildDatabase;

namespace Assets._Code.Building
{
	public class TilePlacementHover : MonoBehaviour
	{
		[SerializeField] private Color m_validColor = Color.green;
		[SerializeField] private Color m_inValidColor = Color.red;
		[SerializeField] private Color m_demolishColor = Color.black;

		[SerializeField] private SpriteRenderer m_renderer;

		[SerializeField] private Tilemap m_groundTileMap;

		private bool m_canBePlaced;
		private bool m_demolishMode;

		public bool CanBePlaced => m_canBePlaced;

		private void Start ()
		{
			Hide();
		}

		public void SetPosition (Vector3Int position, TileRule rule, Tilemap tilemap)
		{
			gameObject.SetActive(true);

			transform.position = tilemap.CellToWorld(position + new Vector3Int(0,0,1));

			if (m_demolishMode)
			{
				m_renderer.sprite = rule == null ? null : rule.Tile.sprite;
				m_renderer.color = m_demolishColor;
				return;
			}

			if(!rule.CanPlace(m_groundTileMap, position))
			{
				m_canBePlaced = false;
			} else if(m_groundTileMap.HasTile(position + Vector3Int.right) ||
				m_groundTileMap.HasTile(position + Vector3Int.left) ||
				m_groundTileMap.HasTile(position + Vector3Int.up) ||
				m_groundTileMap.HasTile(position + Vector3Int.down))
			{
				m_canBePlaced = true;
			} else { m_canBePlaced = false; }


			m_renderer.sprite = rule.Tile.sprite;
			m_renderer.color = m_canBePlaced ? m_validColor : m_inValidColor;
		}
		
		public void Hide ()
		{
			gameObject.SetActive(false);
		}

		public void ToggleDemolishMode ()
		{
			m_demolishMode = !m_demolishMode;
		}
	}
}
