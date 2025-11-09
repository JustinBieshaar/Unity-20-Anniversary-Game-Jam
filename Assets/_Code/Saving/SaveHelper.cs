using Assets._Code._Global;
using System.IO;
using UnityEngine;

namespace Assets._Code.Saving
{
	public static class SaveHelper
	{
		public static string GetSaveFilePath (int slot)
		{
			return Path.Combine(Application.persistentDataPath, $"TilemapWorld{slot}.json");
		}

		public static void DeleteSave (int slot)
		{
			var saveKey = PrefKeys.KEY_SAVE_NAME + slot;
			PlayerPrefs.DeleteKey(saveKey);

#if UNITY_WEBGL && !UNITY_EDITOR
			string key = $"TilemapWorld{slot}";
			if (PlayerPrefs.HasKey(key))
			{
				PlayerPrefs.DeleteKey(key);
				PlayerPrefs.Save();
				Debug.Log($"Deleted save slot {slot} (WebGL)");
			}
			else
			{
				Debug.LogWarning($"No save found in slot {slot} (WebGL)!");
			}
#else

			string path = GetSaveFilePath(slot);
			if (File.Exists(path))
			{
				File.Delete(path);
				Debug.Log($"Deleted save file at slot {slot}: {path}");
			}
			else
			{
				Debug.LogWarning($"No save file found in slot {slot}!");
			}
#endif
		}
	}
}
