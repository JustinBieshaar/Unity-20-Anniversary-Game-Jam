using Assets._Code.Building;
using Assets._Code.Building.Data;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using static Assets._Code.Building.Data.BuildDatabase;

namespace Assets._Code.UI.Building
{
	public class BuildSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private CanvasGroup m_canvasGroup;
		[SerializeField] private RectTransform m_content;
		[SerializeField] private BuildItem m_itemPrefab;

		private List<BuildItem> m_items = new List<BuildItem>();

		private BuildItem m_activeItem = null;

		[SerializeField] private bool m_pointerEnter;
		[SerializeField] private bool m_pointerDown;

		private bool m_isVisible;

		private void Start ()
		{
			Hide(true);
			m_pointerEnter = false;
			m_pointerDown = false;

			CheckVisibility();
		}

		public void Show (float yPos)
		{
			gameObject.SetActive(true);
			m_pointerDown = false;

			/*var pos = transform.position;
			pos.y = yPos;
			transform.position = pos;*/

			CheckVisibility();
		}

		public void Hide (bool ignoreTilePlacer = false)
		{
			for (int i = m_items.Count - 1; i >= 0; i--)
			{
				Destroy(m_items[i].gameObject);
			}

			m_items.Clear();
			gameObject.SetActive(false);
			m_activeItem = null;

			if (ignoreTilePlacer) return;
			TilePlacer.Instance.SetTileToPlace(null);
			TilePlacer.Instance.SetTileMap(null);
		}

		public void SetDatabase(BuildDatabase db)
		{
			for (int i = 0; i < db.Tiles.Count; i++)
			{
				var tile = db.Tiles[i];
				AddItem(tile);
			}
		}

		private void AddItem(TileConfig tile)
		{
			var item = Instantiate(m_itemPrefab, m_content);
			item.Init(tile, OnItemClicked);
			m_items.Add(item);
		}

		private void OnItemClicked (BuildItem item, TileConfig tile)
		{
			m_activeItem?.Deactivate();

			if (m_activeItem == item)
			{
				TilePlacer.Instance.SetTileToPlace(null);
				m_activeItem = null;
				return;
			}

			TilePlacer.Instance.SetTileToPlace(tile);
			m_activeItem = item;
		}

		private void CheckVisibility ()
		{
			if (!m_pointerEnter)
			{
				if (m_isVisible)
				{
					m_canvasGroup.DOFade(0.3f, 0.4f);
				}
				m_isVisible = false;
				return;
			}

			if (!m_pointerDown)
			{
				if (!m_isVisible)
				{
					m_canvasGroup.DOFade(1.0f, 0.4f);
				}
				m_isVisible = true;
				return;
			}
		}

		void Update ()
		{
			if (m_isVisible) return;

			if (Input.GetMouseButtonDown(0))
			{
				m_pointerDown = true;
				CheckVisibility();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				m_pointerDown = false;
				CheckVisibility();
			}
		}

		public void OnPointerEnter (PointerEventData eventData)
		{
			m_pointerEnter = true;
			CheckVisibility();
		}

		public void OnPointerExit (PointerEventData eventData)
		{
			m_pointerEnter = false;
			CheckVisibility();
		}
	}
}
