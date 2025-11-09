using Assets._Code.Saving;
using Assets._Code.Sound;
using Assets._Code.UI.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets._Code.UI
{
	public class BackButton : MonoBehaviour
	{
		[SerializeField] private OverlayYesNo m_yesNoOverlay;

		private void Start ()
		{
			GetComponent<Button>().onClick.AddListener(() =>
			{
				m_yesNoOverlay.Show((bool yes) =>
				{
					if (!yes) return;

					SaveManager.Instance.SaveTilemaps();
					SoundManager.Instance.Reset();
					SceneManager.LoadScene(0);
				});
			});
		}
	}
}
