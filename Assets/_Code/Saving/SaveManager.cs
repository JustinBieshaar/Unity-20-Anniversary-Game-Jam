using Assets._Code._Global;
using Assets._Code.Blobs;
using Assets._Code.Building.Data;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Saving
{
	[System.Serializable]
	public class TilemapSaveData
	{
		[System.Serializable]
		public class TileData
		{
			public Vector3Int cell;
			public string tileID;
		}

		public List<TileData> groundTiles = new List<TileData>();
		public List<TileData> environmentTiles = new List<TileData>();
	}

	public class SaveManager : MonoBehaviour
	{
		public static SaveManager Instance { get; private set; }

		[Header("Tilemaps")]
		[SerializeField] private Tilemap m_groundTilemap;
		[SerializeField] private Tilemap m_environmentTilemap;
		[SerializeField] private TileConfigDatabase m_tileDatabase;

		[Header("Auto Save")]
		[SerializeField] private float m_autoSaveInterval = 300f; // 5 min
		private float m_autoSaveTimer;

		private int m_currentSlot;

		private void Awake ()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);

			m_currentSlot = PlayerPrefs.GetInt(PrefKeys.KEY_CURRENT_SAVE);
		}

		private void Start ()
		{
			LoadTilemaps();
		}

		private void Update ()
		{
			m_autoSaveTimer += Time.deltaTime;
			if (m_autoSaveTimer >= m_autoSaveInterval)
			{
				m_autoSaveTimer = 0f;
				SaveTilemaps();
			}

			if (Input.GetKeyDown(KeyCode.F5))
			{
				SaveTilemaps();
			}
			if (Input.GetKeyDown(KeyCode.F6))
			{
				LoadTilemaps();
			}
		}

		#region Save / Load

		public void SaveTilemaps ()
		{
			TilemapSaveData saveData = new TilemapSaveData();
			SaveTilemap(m_groundTilemap, saveData.groundTiles);
			SaveTilemap(m_environmentTilemap, saveData.environmentTiles);

			string json = JsonUtility.ToJson(saveData, true);

#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString($"TilemapWorld{slot}", json);
            PlayerPrefs.Save();
            Debug.Log($"Tilemaps saved to slot {slot} (WebGL)");
#else
			File.WriteAllText(SaveHelper.GetSaveFilePath(m_currentSlot), json);
			Debug.Log($"Tilemaps saved to slot {m_currentSlot}");
#endif
		}

		public void LoadTilemaps ()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
            string key = $"TilemapWorld{slot}";
            if (!PlayerPrefs.HasKey(key))
            {
                Debug.LogWarning($"Save slot {slot} not found (WebGL)!");
                return;
            }
            string json = PlayerPrefs.GetString(key);
#else
			string path = SaveHelper.GetSaveFilePath(m_currentSlot);
			if (!File.Exists(path))
			{
				Debug.LogWarning($"Save slot {m_currentSlot} not found!");
				return;
			}
			string json = File.ReadAllText(path);
#endif

			TilemapSaveData saveData = JsonUtility.FromJson<TilemapSaveData>(json);

			m_groundTilemap.ClearAllTiles();
			m_environmentTilemap.ClearAllTiles();

			LoadTilemap(saveData.groundTiles, m_groundTilemap);
			LoadTilemap(saveData.environmentTiles, m_environmentTilemap);

			Debug.Log($"Tilemaps loaded from slot {m_currentSlot}");

			BlobManager.Instance.Distribute();
		}

		#endregion

		#region Helpers

		private void SaveTilemap (Tilemap tilemap, List<TilemapSaveData.TileData> list)
		{
			list.Clear();
			BoundsInt bounds = tilemap.cellBounds;

			foreach (var pos in bounds.allPositionsWithin)
			{
				Tile tile = tilemap.GetTile<Tile>(pos);
				if (tile == null) continue;

				string tileID = m_tileDatabase.GetTileID(tile); // TileBase -> string
				if(string.IsNullOrEmpty(tileID)) continue;
				list.Add(new TilemapSaveData.TileData { cell = pos, tileID = tileID });
			}
		}

		private void LoadTilemap (List<TilemapSaveData.TileData> list, Tilemap tilemap)
		{
			foreach (var data in list)
			{
				TileBase tile = m_tileDatabase.GetTileByID(data.tileID); // string -> TileBase
				if (tile != null)
					tilemap.SetTile(data.cell, tile);
			}
		}

		#endregion
	}
}
