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
		private TileRule m_tileToPlace;

		public static TilePlacer Instance => s_instance;
		private static TilePlacer s_instance; // I hate this..

		private List<Vector3Int> m_animPositions = new List<Vector3Int>();
		private bool m_demolishMode;

		public bool IsInDemolishMode => m_demolishMode;

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

		public void SetTileToPlace (TileRule tile)
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
			if(m_tilemap == null || m_tileToPlace == null) return;

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

			m_tilePlacementHover.SetPosition(cellPos, new TileRule() { Tile = tile }, m_ground);

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

		public void ToggleDemolish ()
		{
			m_currentDemolishTileMap = null;
			m_demolishMode = !m_demolishMode;
			m_tilePlacementHover.ToggleDemolishMode();
		}
	}
}