using Assets._Code.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Code
{
	public class BackgroundMusicPlayer : MonoBehaviour
	{
		[SerializeField] private AudioClip m_clip;

		private void Start ()
		{
			SoundManager.Instance.PlayBGM(m_clip);
		}
	}
}
