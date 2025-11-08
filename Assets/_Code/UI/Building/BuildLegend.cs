using Assets._Code.Building;
using Assets._Code.Building.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Assets._Code.UI.Building
{
	public class BuildLegend : MonoBehaviour
	{
		[SerializeField] private BuildSelector m_buildSelector;

		[SerializeField] private List<ButtonMap> m_legendButtons;

		[SerializeField] private Color m_activeColor;

		[Serializable]
		public class ButtonMap
		{
			public Button Button;
			public BuildDatabase BuildDatabase;
			public Tilemap TileMap;

			private Color m_normalColor;

			public void Init ()
			{
				m_normalColor = Button.image.color;
			}

			public void Deactivate ()
			{
				Button.image.color = m_normalColor;
			}

			public void Activate(Color color)
			{
				Button.image.color = color;
			}
		}

		private ButtonMap m_activeButtonMap = null;

		private void Start ()
		{
			foreach (var buttonMap in m_legendButtons)
			{
				buttonMap.Init();
				var selectedMap = buttonMap;
				buttonMap.Button.onClick.AddListener(() => { OnLegendSelected(selectedMap); });
			}
		}

		private void OnLegendSelected (ButtonMap buttonMap)
		{
			m_buildSelector.Hide();
			m_activeButtonMap?.Deactivate();

			if (m_activeButtonMap == buttonMap)
			{
				m_activeButtonMap = null;
				return;
			}

			m_activeButtonMap = buttonMap;
			buttonMap.Activate(m_activeColor);

			m_buildSelector.Show(buttonMap.Button.transform.position.y);
			m_buildSelector.SetDatabase(buttonMap.BuildDatabase);

			TilePlacer.Instance.SetTileMap(buttonMap.TileMap);
		}
	}
}
