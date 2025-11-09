using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets._Code.UI
{
	public class UITip : MonoBehaviour
	{
		public static UITip Instance { get; private set; }

		[SerializeField] private TextMeshProUGUI m_text;

		private void Awake ()
		{
			Instance = this;	
		}

		public void Hide ()
		{
			if(m_text.color.a > 0)
				m_text.DOFade(0, 0.5f);
		}
	}
}
