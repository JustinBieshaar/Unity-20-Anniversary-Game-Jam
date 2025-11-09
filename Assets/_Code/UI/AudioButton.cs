using Assets._Code.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Code.UI
{
	
	public class AudioButton : MonoBehaviour, IPointerDownHandler
	{
		[SerializeField] private AudioClip m_clip;
		[SerializeField] private float m_volume = 0.25f;
		[SerializeField] private float m_pitchRange = 0.25f;

		private Button m_button;

		public void OnPointerDown (PointerEventData eventData)
		{
			SoundManager.Instance.PlaySFX(m_clip, m_volume, m_pitchRange);
		}

		private void Start ()
		{
		}
	}
}
