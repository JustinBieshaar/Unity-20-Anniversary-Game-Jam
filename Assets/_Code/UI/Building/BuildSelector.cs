using Assets._Code.Building;
using Assets._Code.Building.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.UI.Building
{
	public class BuildSelector : MonoBehaviour
	{
		[SerializeField] private RectTransform m_content;
		[SerializeField] private BuildItem m_itemPrefab;

		private List<BuildItem> m_items = new List<BuildItem>();

		private BuildItem m_activeItem = null;

		private void Awake ()
		{
			Hide(true);
		}

		public void Show (float yPos)
		{
			gameObject.SetActive(true);

			var pos = transform.position;
			pos.y = yPos;
			transform.position = pos;
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

		private void AddItem(Tile tile)
		{
			var item = Instantiate(m_itemPrefab, m_content);
			item.Init(tile, OnItemClicked);
			m_items.Add(item);
		}

		private void OnItemClicked (BuildItem item, Tile tile)
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
	}
}
