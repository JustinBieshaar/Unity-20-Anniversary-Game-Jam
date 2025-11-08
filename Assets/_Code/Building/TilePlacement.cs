using Assets._Code.Building.Data;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Assets._Code.Building.Data.BuildDatabase;

namespace Assets._Code.Building
{
	public class TilePlacer : MonoBehaviour
	{
		[SerializeField] private GameObject m_placeEffectPrefab; // small sprite or placeholder object
		[SerializeField] private GameObject m_hoverEffectPrefab; // small sprite or placeholder object

		[SerializeField] private Tilemap m_ground;
		[SerializeField] private Tilemap m_environment;

		[SerializeField] private TilePlacementHover m_tilePlacementHover;

		private Tilemap m_tilemap;
		private TileConfig m_tileToPlace;

		public static TilePlacer Instance => s_instance;
		private static TilePlacer s_instance; // I hate this..

		private List<Vector3Int> m_animPositions = new List<Vector3Int>();
		private bool m_demolishMode;
		private bool m_isFieldPlaceMode;

		private Vector3Int? m_dragStartPos;

		public bool IsInDemolishMode => m_demolishMode;
		public bool IsInFieldMode => m_isFieldPlaceMode;

		private Tilemap m_currentDemolishTileMap;

		private void Start ()
		{
			s_instance = this;
		}

		public void Stop ()
		{
			m_tilemap = null;
			m_tileToPlace = null;
			m_tilePlacementHover.Hide();
		}

		public void SetTileToPlace (TileConfig tile)
		{
			m_tileToPlace = tile;
		}

		public void SetTileMap (Tilemap map)
		{
			m_tilemap = map;
		}

		void Update ()
		{
			if (m_demolishMode)
			{
				CheckDemolish();
				return;
			}

			if (m_isFieldPlaceMode)
			{
				HandleFieldPlacement();
				return;
			}

			if (m_tilemap == null || m_tileToPlace == null) return;

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0;
			Vector3Int cellPos = m_tilemap.WorldToCell(mouseWorldPos);
			cellPos.z = 0;

			m_tilePlacementHover.SetPosition(cellPos, m_tileToPlace, m_tilemap);

			if (!m_tilePlacementHover.CanBePlaced)
				return;

			if (Input.GetMouseButton(0))
			{
				// Only place if tile is empty
				if (m_tilemap.GetTile(cellPos) != m_tileToPlace.Tile && !m_animPositions.Contains(cellPos))
				{
					if (m_tilemap.HasTile(cellPos))
					{
						m_tilemap.SetTile(cellPos, null);
					}
					StartCoroutine(AnimateTilePlacement(cellPos));
				}

			}
		}

		private void HandleFieldPlacement ()
		{
			if (m_tilemap == null || m_tileToPlace == null) return;

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0;
			Vector3Int cellPos = m_tilemap.WorldToCell(mouseWorldPos);
			cellPos.z = 0;

			if (Input.GetMouseButtonDown(0))
			{
				m_dragStartPos = cellPos;
			}

			if (m_dragStartPos.HasValue)
			{
				// Draw rectangle preview
				var start = m_dragStartPos.Value;
				var min = Vector3Int.Min(start, cellPos);
				var max = Vector3Int.Max(start, cellPos);

				m_tilePlacementHover.SetRectPreview(min, max, m_tileToPlace, m_tilemap);
			} else
			{
				m_tilePlacementHover.SetPosition(cellPos, m_tileToPlace, m_tilemap);
			}

			if (Input.GetMouseButtonUp(0) && m_dragStartPos.HasValue)
			{
				m_tilePlacementHover.ClearPreview();
				if (!m_tilePlacementHover.CanBePlaced)
				{
					m_dragStartPos = null;
					return;
				}

				var start = m_dragStartPos.Value;
				var min = Vector3Int.Min(start, cellPos);
				var max = Vector3Int.Max(start, cellPos);

				StartCoroutine(PlaceFieldTiles(min, max));

				m_dragStartPos = null;
			}
		}

		private void CheckDemolish ()
		{
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseWorldPos.z = 0;
			Vector3Int cellPos = m_ground.WorldToCell(mouseWorldPos);
			cellPos.z = 0;

			var tile = m_ground.GetTile<Tile>(cellPos);
			if (m_environment.HasTile(cellPos))
			{ 
				tile = m_environment.GetTile<Tile>(cellPos);
			}

			m_tilePlacementHover.SetPosition(cellPos, new TileConfig(tile), m_ground);

			if (Input.GetMouseButton(0))
			{
				if (m_currentDemolishTileMap == null)
				{
					if (m_environment.HasTile(cellPos))
					{
						m_currentDemolishTileMap = m_environment;
					}
					else if (m_ground.HasTile(cellPos))
					{
						m_currentDemolishTileMap = m_ground;
					}
				}
				
				m_currentDemolishTileMap?.SetTile(cellPos, null);
			} else if (Input.GetMouseButtonUp(0))
			{
				m_currentDemolishTileMap = null;
			}
		}

		private System.Collections.IEnumerator AnimateTilePlacement (Vector3Int cellPos)
		{
			m_animPositions.Add(cellPos);
			Vector3 worldPos = m_tilemap.GetCellCenterWorld(cellPos);

			// Spawn temporary object for animation
			GameObject animObj = Instantiate(m_placeEffectPrefab, worldPos, Quaternion.identity);
			animObj.transform.localScale = Vector3.zero;
			var sr = animObj.GetComponent<SpriteRenderer>();
			sr.sprite = m_tileToPlace.Tile.sprite;
			sr.sortingOrder = m_tilemap.GetComponent<TilemapRenderer>().sortingOrder;

			// Animate pop-in
			animObj.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

			// Wait for animation
			yield return new WaitForSeconds(0.25f);

			// Apply actual tile
			m_tilemap.SetTile(cellPos, m_tileToPlace.Tile);

			// Remove the animation object
			Destroy(animObj);
			m_animPositions.Remove(cellPos);
		}

		private System.Collections.IEnumerator PlaceFieldTiles (Vector3Int min, Vector3Int max)
		{
			for (int x = min.x; x <= max.x; x++)
			{
				for (int y = min.y; y <= max.y; y++)
				{
					Vector3Int pos = new Vector3Int(x, y, 0);
					if (m_tilemap.GetTile(pos) != m_tileToPlace.Tile && !m_animPositions.Contains(pos))
					{
						StartCoroutine(AnimateTilePlacement(pos));
						yield return new WaitForSeconds(0.01f); // small stagger for performance/look
					}
				}
			}
		}

		public void ToggleDemolish ()
		{
			m_currentDemolishTileMap = null;
			m_demolishMode = !m_demolishMode;
			m_tilePlacementHover.ToggleDemolishMode();
		}

		public void ToggleFieldPlaceMode ()
		{
			m_isFieldPlaceMode = !m_isFieldPlaceMode;

			m_tilePlacementHover.ClearPreview();
		}
	}
}