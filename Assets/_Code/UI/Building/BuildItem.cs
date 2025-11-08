using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Assets._Code.Building.Data.BuildDatabase;

namespace Assets._Code.UI.Building
{
	public class BuildItem : MonoBehaviour
	{
		[SerializeField] private Button m_button;
		[SerializeField] private Image m_icon;
		[SerializeField] private Color m_activeColor;

		private TileRule m_tile;
		private Action<BuildItem, TileRule> m_onClick;
		private Color m_normalColor;

		private void Start ()
		{
			m_normalColor = m_button.image.color;
			m_button.onClick.AddListener(ButtonClicked);
		}

		private void ButtonClicked ()
		{
			m_button.image.color = m_activeColor;
			m_onClick?.Invoke(this, m_tile);
		}

		public void Init(TileRule tile, Action<BuildItem, TileRule> onClick)
		{
			m_tile = tile;

			m_icon.sprite = tile.Tile.sprite;
			m_onClick = onClick;
		}

		public void Deactivate ()
		{
			Debug.Log("Deactivated item");
			m_button.image.color = m_normalColor;
		}
	}
}
