using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Code.Saving
{
	public class SaveButton : MonoBehaviour
	{
		[SerializeField] private Button m_button;

		private void Start ()
		{
			m_button.onClick.AddListener(() =>
			{
				SaveManager.Instance.SaveTilemaps();
			});
		}
	}
}
