using Assets._Code.Blobs;
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

		[SerializeField] private Button m_demolishButton;
		[SerializeField] private Button m_fieldPlacementModeButton;

		[SerializeField] private Color m_activeColor;
		private Color m_demolishColor;
		private Color m_fieldModeColor;

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
			m_demolishColor = m_demolishButton.image.color;
			m_fieldModeColor = m_fieldPlacementModeButton.image.color;

			foreach (var buttonMap in m_legendButtons)
			{
				buttonMap.Init();
				var selectedMap = buttonMap;
				buttonMap.Button.onClick.AddListener(() => { OnLegendSelected(selectedMap); });
			}

			m_demolishButton.onClick.AddListener(ToggleDemolish);
			m_fieldPlacementModeButton.onClick.AddListener(ToggleFieldMode);
		}

		private void ToggleDemolish ()
		{
			TilePlacer.Instance.Stop();
			TilePlacer.Instance.ToggleDemolish();
			
			m_buildSelector.Hide();

			m_demolishButton.image.color = TilePlacer.Instance.IsInDemolishMode ? m_activeColor : m_demolishColor;

			m_activeButtonMap?.Deactivate();
			m_activeButtonMap = null;
		}

		private void ToggleFieldMode ()
		{
			TilePlacer.Instance.ToggleFieldPlaceMode();
			m_fieldPlacementModeButton.image.color = TilePlacer.Instance.IsInFieldMode ? m_activeColor : m_fieldModeColor;
		}

		private void OnLegendSelected (ButtonMap buttonMap)
		{
			TilePlacer.Instance.Stop();
			if (TilePlacer.Instance.IsInDemolishMode)
			{
				TilePlacer.Instance.ToggleDemolish();
				m_demolishButton.image.color = m_demolishColor;
			}

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
