using Assets._Code.Sound;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Code.UI.Menu
{
	public class OverlayStartGame : MonoBehaviour
	{
		[SerializeField] private AudioClip m_startGameSound;
		[SerializeField] private Image m_image;

		public static OverlayStartGame Instance { get; private set; }

		private void Start ()
		{
			Instance = this;

			gameObject.SetActive(false);
			var cacheColor = m_image.color;
			cacheColor.a = 0;
			m_image.color = cacheColor;
		}

		public void StartGame ()
		{
			gameObject.SetActive(true);
			SoundManager.Instance.FadeBGM();
			SoundManager.Instance.PlaySFX(m_startGameSound);

			m_image.DOFade(1, 0.5f);

			DOVirtual.DelayedCall(5f, () =>
			{
				SoundManager.Instance.Reset();
				SceneManager.LoadScene(1);
			});
		}
	}
}
