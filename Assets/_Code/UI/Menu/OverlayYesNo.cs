using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Code.UI.Menu
{
	internal class OverlayYesNo : MonoBehaviour
	{
		[SerializeField] private Button m_btnYes;
		[SerializeField] private Button m_close;

		[Header("Animation references")]
		[SerializeField] private Image m_overlay;
		[SerializeField] private RectTransform m_content;

		private float m_delay = 0.3f;

		private bool m_yes;
		private Action<bool> m_onClose;

		private void Start ()
		{
			m_close.onClick.AddListener(() =>
			{
				m_yes = false;
				AnimateAway();
			});
			m_btnYes.onClick.AddListener(() =>
			{
				m_yes = true;
				AnimateAway();
			});
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

				m_onClose?.Invoke(m_yes);
			});
			m_content.DOAnchorPosY(-500, 0.5f);
		}

		public void Show (Action<bool> onClose)
		{
			m_onClose = onClose;

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
