using Assets._Code.Sound;
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
		private void Start ()
		{
			GetComponent<Button>().onClick.AddListener(() =>
			{
				SoundManager.Instance.Reset();
				SceneManager.LoadScene(0);
			});
		}
	}
}
