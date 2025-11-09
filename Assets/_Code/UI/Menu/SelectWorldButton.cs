using Assets._Code._Global;
using Assets._Code.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Code.UI.Menu
{
	public class SelectWorldButton : MonoBehaviour
	{
		[SerializeField] private int m_saveIndex;
		[SerializeField] private TextMeshProUGUI m_text;
		[SerializeField] private WorldNameDialog m_worldNameDialog;
		[SerializeField] private Button m_deleteButton;

		private bool m_hasSave = false;

		private void Start ()
		{
			GetComponent<Button>().onClick.AddListener(() =>
			{
				PlayerPrefs.SetInt(PrefKeys.KEY_CURRENT_SAVE, m_saveIndex);
				if (!m_hasSave)
				{
					m_worldNameDialog.Show();
				} else
				{
					// start game
					OverlayStartGame.Instance.StartGame();
				}
			});

			m_deleteButton.onClick.AddListener(() =>
			{
				SaveHelper.DeleteSave(m_saveIndex);
				UpdateVisuals();
			});

			UpdateVisuals();
		}

		private void UpdateVisuals ()
		{
			if (PlayerPrefs.HasKey(PrefKeys.KEY_SAVE_NAME + m_saveIndex))
			{
				m_hasSave = true;
				m_text.text = PlayerPrefs.GetString(PrefKeys.KEY_SAVE_NAME + m_saveIndex);
				m_deleteButton.gameObject.SetActive(true);
			}
			else
			{
				m_hasSave = false;
				m_deleteButton.gameObject.SetActive(false);
				m_text.text = "NEW";
			}
		}
	}
}
