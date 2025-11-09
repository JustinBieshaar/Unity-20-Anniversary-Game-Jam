

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Code.Sound
{
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager Instance { get; private set; }

		[Header("Audio Sources")]
		[SerializeField] private AudioSource m_sfxSourcePrefab; // prefab for one-shot SFX
		[SerializeField] private AudioSource m_bgmSource;        // dedicated source for BGM

		[Header("Settings")]
		[SerializeField] private float m_sfxVolume = 1f;
		[SerializeField] private float m_bgmVolume = 0.5f;

		private List<AudioSource> m_sfxSources = new();

		private void Awake ()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
		}

		#region SFX

		/// <summary>
		/// Plays a one-shot sound effect at the manager’s position
		/// </summary>
		public void PlaySFX (AudioClip clip, float volume = 1f, float pitchVariance = 0.1f)
		{
			if (clip == null) return;

			// Get an AudioSource from pool or instantiate a new one
			AudioSource source = GetAvailableSFXSource();
			source.clip = clip;
			source.volume = volume * m_sfxVolume;
			source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
			source.Play();
		}

		private AudioSource GetAvailableSFXSource ()
		{
			foreach (var src in m_sfxSources)
			{
				if (!src.isPlaying)
					return src;
			}

			// none available, create a new one
			AudioSource newSrc = Instantiate(m_sfxSourcePrefab, transform);
			m_sfxSources.Add(newSrc);
			return newSrc;
		}

		#endregion

		#region BGM

		/// <summary>
		/// Plays background music, looping. Stops current track if one is playing.
		/// </summary>
		public void PlayBGM (AudioClip clip, bool loop = true)
		{
			if (clip == null || m_bgmSource == null) return;

			if (m_bgmSource.clip == clip && m_bgmSource.isPlaying)
				return; // already playing this track

			m_bgmSource.Stop();
			m_bgmSource.clip = clip;
			m_bgmSource.loop = loop;
			m_bgmSource.volume = m_bgmVolume;
			m_bgmSource.Play();
		}

		public void StopBGM ()
		{
			if (m_bgmSource.isPlaying)
				m_bgmSource.Stop();
		}

		public void SetBGMVolume (float volume)
		{
			m_bgmVolume = Mathf.Clamp01(volume);
			if (m_bgmSource != null)
				m_bgmSource.volume = m_bgmVolume;
		}

		public void FadeBGM (float to = 0.0f, float duration = 1.0f)
		{
			m_bgmSource.DOFade(to, duration);
		}

		public void Reset ()
		{
			Instance = null;
		}

		#endregion
	}
}

