using Assets._Code._Global;
using DG.Tweening;
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
	public class WorldNameDialog : MonoBehaviour
	{
		[SerializeField] private TMP_InputField m_inputField;
		[SerializeField] private Button m_start;
		[SerializeField] private Button m_close;

		[Header("Animation references")]
		[SerializeField] private Image m_overlay;
		[SerializeField] private RectTransform m_content;

		private float m_delay = 0.3f;

		private void Start ()
		{
			//Hide();

			m_inputField.text = "";
			m_inputField.onValueChanged.AddListener(ValidateName);

			m_start.interactable = false;

			m_close.onClick.AddListener(AnimateAway);
			m_start.onClick.AddListener(StartGame);
		}

		private void StartGame ()
		{
			int saveIndex = PlayerPrefs.GetInt(PrefKeys.KEY_CURRENT_SAVE, 0);
			PlayerPrefs.SetString(PrefKeys.KEY_SAVE_NAME + saveIndex, m_inputField.text);

			OverlayStartGame.Instance.StartGame();
		}

		private void ValidateName (string arg0)
		{
			m_start.interactable = arg0.Length > 2;
		}

		public void Hide ()
		{
			transform.gameObject.SetActive(false);
		}

		public void AnimateAway ()
		{
			m_close.interactable = false;
			m_overlay.DOFade(0.0f, 0.5f).SetDelay(m_delay).OnComplete(() =>
			{
				transform.gameObject.SetActive(false);
			});
			m_content.DOAnchorPosY(-500, 0.5f);
		}

		public void Show ()
		{
			m_close.interactable = true;
			transform.gameObject.SetActive(true);
			var anchoredPosition = m_content.anchoredPosition;
			anchoredPosition.y = -500;
			m_content.anchoredPosition = anchoredPosition;

			var cacheColor = m_overlay.color;
			cacheColor.a = 0;

			m_overlay.color = cacheColor;

			m_overlay.DOFade(0.7f, 0.5f);
			m_content.DOAnchorPosY(0, 0.5f).SetDelay(m_delay);
		}
	}
}
