using Assets._Code.Building.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
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
		[SerializeField] private Tilemap m_environmentTileMap;

		[SerializeField] private GameObject m_previewTilePrefab; // simple prefab with a SpriteRenderer
		private readonly List<SpriteRenderer> m_previewTiles = new List<SpriteRenderer>();

		private bool m_canBePlaced;
		private bool m_demolishMode;

		public bool CanBePlaced => m_canBePlaced;

		private void Start ()
		{
			Hide();
		}

		public void SetPosition (Vector3Int position, TileConfig rule, Tilemap tilemap)
		{
			gameObject.SetActive(true);

			transform.position = tilemap.CellToWorld(position + new Vector3Int(0,0,1));

			if (m_demolishMode)
			{
				m_renderer.sprite = rule == null || rule.Tile == null ? null : rule.Tile.sprite;
				m_renderer.color = m_demolishColor;
				return;
			}

			if(!rule.CanPlace(m_groundTileMap, m_environmentTileMap, position))
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

			foreach (var sr in m_previewTiles)
				sr.gameObject.SetActive(false);
		}

		public void ToggleDemolishMode ()
		{
			m_demolishMode = !m_demolishMode;
		}

		public void SetRectPreview (Vector3Int min, Vector3Int max, TileConfig TileConfig, Tilemap tilemap)
		{
			if (TileConfig == null || TileConfig.Tile == null)
				return;

			gameObject.SetActive(true);

			int neededCount = (max.x - min.x + 1) * (max.y - min.y + 1);
			//Debug.Log($"Needed count: {neededCount}, existing: {m_previewTiles.Count}");

			// Create new preview sprites if needed
			while (m_previewTiles.Count < neededCount)
			{
				var obj = Instantiate(m_previewTilePrefab, transform);
				var sr = obj.GetComponent<SpriteRenderer>();
				sr.gameObject.SetActive(false);
				m_previewTiles.Add(sr);
			}

			// Hide any unused ones
			for (int i = 0; i < m_previewTiles.Count; i++)
				m_previewTiles[i].gameObject.SetActive(i < neededCount);

			m_canBePlaced = true;
			bool hasNeighboringTile = false;
			int index = 0;
			for (int x = min.x; x <= max.x; x++)
			{
				for (int y = min.y; y <= max.y; y++)
				{
					Vector3Int cell = new Vector3Int(x, y, 0);
					SpriteRenderer sr = m_previewTiles[index++];
					sr.gameObject.SetActive(true);

					sr.sprite = TileConfig.Tile.sprite;
					sr.transform.position = tilemap.GetCellCenterWorld(cell);
					sr.sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;

					bool canPlace = TileConfig.CanPlace(m_groundTileMap, m_environmentTileMap, cell);

					if (m_groundTileMap.HasTile(cell + Vector3Int.right) ||
						m_groundTileMap.HasTile(cell + Vector3Int.left) ||
						m_groundTileMap.HasTile(cell + Vector3Int.up) ||
						m_groundTileMap.HasTile(cell + Vector3Int.down))
					{
						hasNeighboringTile = true;
					}

					sr.color = (canPlace ? m_validColor : m_inValidColor) * new Color(1, 1, 1, 0.7f);

					if (!canPlace)
					{
						m_canBePlaced = false;
					}
				}
			}
			if(m_canBePlaced && !hasNeighboringTile)
			{
				m_canBePlaced = false;
				foreach (var item in m_previewTiles)
				{
					item.color = m_inValidColor;
				}
			}

			m_renderer.gameObject.SetActive(false);
		}

		public void ClearPreview ()
		{
			foreach (var item in m_previewTiles)
			{
				Destroy(item.gameObject);
			}
			m_previewTiles.Clear();
			m_renderer.gameObject.SetActive(true);
		}
	}
}
