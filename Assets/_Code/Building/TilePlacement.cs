using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
		private Tile m_tileToPlace;

		public static TilePlacer Instance => s_instance;
		private static TilePlacer s_instance; // I hate this..

		private List<Vector3Int> m_animPositions = new List<Vector3Int>();

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
				if (m_tilemap.GetTile(cellPos) != m_tileToPlace && !m_animPositions.Contains(cellPos))
				{
					if (m_tilemap.HasTile(cellPos))
					{
						m_tilemap.SetTile(cellPos, null);
					}
					StartCoroutine(AnimateTilePlacement(cellPos));
				}

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
			sr.sprite = m_tileToPlace.sprite;
			sr.sortingOrder = m_tilemap.GetComponent<TilemapRenderer>().sortingOrder;

			// Animate pop-in
			animObj.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

			// Wait for animation
			yield return new WaitForSeconds(0.25f);

			// Apply actual tile
			m_tilemap.SetTile(cellPos, m_tileToPlace);

			// Remove the animation object
			Destroy(animObj);
			m_animPositions.Remove(cellPos);
		}
	}
}