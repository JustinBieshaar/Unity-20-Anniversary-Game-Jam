using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Code.UI
{
	public class UIClockHand : MonoBehaviour
	{
		[Header("Assign your UI Image Transforms")]
		[SerializeField] private RectTransform m_hourHand;
		[SerializeField] private RectTransform m_minuteHand;

		[SerializeField] private CanvasGroup m_canvas;

		void Start ()
		{
			// Get current system time
			DateTime now = DateTime.Now;

			// Calculate hour and minute angles
			// 360° per 12 hours → 30° per hour
			// 360° per 60 minutes → 6° per minute
			float minuteAngle = 90f - (now.Minute * 6f);
			float hourAngle = 90f - ((now.Hour % 12) * 30f + now.Minute * 0.5f);

			// Apply rotation to UI Images
			if (m_hourHand != null)
				m_hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle);
			if (m_minuteHand != null)
				m_minuteHand.localRotation = Quaternion.Euler(0, 0, minuteAngle);
		}

		public void AnimateAway (TweenCallback onComplete = null)
		{
			float duration = 2.2f;

			// Random small rotation & fly-away for each hand
			if (m_hourHand != null)
			{
				m_hourHand.DOLocalRotate(
					new Vector3(0, 0, m_hourHand.localEulerAngles.z - UnityEngine.Random.Range(100, 400)),
					duration,
					RotateMode.FastBeyond360
				).SetEase(Ease.OutSine);

				m_hourHand.DOLocalMove(new Vector3(0, -900), 1f
				).SetEase(Ease.InCubic).SetDelay(duration);
			}

			if (m_minuteHand != null)
			{
				m_minuteHand.DOLocalRotate(
					new Vector3(0, 0, m_minuteHand.localEulerAngles.z - UnityEngine.Random.Range(800, 1200)),
					duration,
					RotateMode.FastBeyond360
				).SetEase(Ease.OutSine);

				m_minuteHand.DOLocalMove(new Vector3(0, -900), 1f
				).SetEase(Ease.InCubic).SetDelay(duration);
			}

			// Optionally destroy the GameObject after the animation
			m_canvas.DOFade(0, 1f).SetDelay(duration + 1 + 0.2f).OnComplete(onComplete);
		}
	}
}
