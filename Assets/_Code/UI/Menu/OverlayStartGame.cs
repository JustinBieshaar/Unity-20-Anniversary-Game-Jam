using Assets._Code.Sound;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Code.UI.Menu
{
	public class OverlayStartGame : MonoBehaviour
	{
		[SerializeField] private AudioClip m_startGameSound;
		[SerializeField] private Image m_image;

		[SerializeField] private UIClockHand m_clock;
		[SerializeField] private CanvasGroup m_clockCanvas;

		[Header("Text animation sequence")]
		[SerializeField] private List<TextMeshProUGUI> m_texts;
		[SerializeField] private List<TextMeshProUGUI> m_subTexts;
		[SerializeField] private float m_textFadeDuration = 0.75f;
		[SerializeField] private float m_textHoldDuration = 2.0f;

		public static OverlayStartGame Instance { get; private set; }

		private void Start ()
		{
			Instance = this;

			gameObject.SetActive(false);
			var cacheColor = m_image.color;
			cacheColor.a = 0;
			m_image.color = cacheColor;

			m_clockCanvas.alpha = 0;

			foreach (var item in m_texts)
			{
				var color = item.color;
				color.a = 0;
				item.color = color;
			}

			foreach (var item in m_subTexts)
			{
				var color = item.color;
				color.a = 0;
				item.color = color;
			}
		}

		public void StartGameSequence ()
		{
			gameObject.SetActive(true);
			SoundManager.Instance.FadeBGM();
			SoundManager.Instance.PlaySFX(m_startGameSound);

			m_image.DOFade(1, 0.5f);
			m_clockCanvas.DOFade(1, 0.5f).SetDelay(0.2f).OnComplete(() =>
			{
				DOVirtual.DelayedCall(0.5f, () =>
				{
					m_clock.AnimateAway(()=> 
					{
						StartTextSequence();
						StartSubTextsSequence();
					});
				});
			});
		}

		private void StartTextSequence ()
		{
			Sequence textsSequence = DOTween.Sequence();

			for (int i = 0; i < m_texts.Count; i++)
			{
				textsSequence.Append(m_texts[i].DOFade(1, m_textFadeDuration));

				textsSequence.AppendInterval(m_textHoldDuration);

				textsSequence.Append(m_texts[i].DOFade(0, m_textFadeDuration));
			}

			textsSequence.AppendInterval(1.0f);

			textsSequence.OnComplete(StartGame);

			textsSequence.Play();
		}

		private void StartSubTextsSequence ()
		{
			float delay = 0.2f;
			float addedDuration = 0.2f;
			Sequence textsSequence = DOTween.Sequence();

			for (int i = 0; i < m_subTexts.Count; i++)
			{
				textsSequence.AppendInterval(delay);
				textsSequence.Append(m_subTexts[i].DOFade(1, m_textFadeDuration + addedDuration));

				textsSequence.AppendInterval(m_textHoldDuration - delay - addedDuration);

				textsSequence.Append(m_subTexts[i].DOFade(0, m_textFadeDuration));
			}

			textsSequence.AppendInterval(1.0f);

			textsSequence.Play();
		}

		private void StartGame ()
		{
			SoundManager.Instance.Reset();
			SceneManager.LoadScene(1);
		}
	}
}
