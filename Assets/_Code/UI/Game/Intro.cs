using Assets._Code.Sound;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Code.UI.Game
{
	public class Intro : MonoBehaviour
	{
		[SerializeField] private Image m_image;
		private void Start ()
		{
			var imgColor = m_image.color;
			imgColor.a = 1;
			m_image.color = imgColor;

			m_image.DOFade(0, 2f).SetDelay(0.5f).OnComplete(() =>
			{
				gameObject.SetActive(false);
			});

			SoundManager.Instance.FadeBGM(1.5f);
		}
	}
}
