using Assets._Code.Building.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets._Code.Blobs.Data
{
	[CreateAssetMenu(fileName = "BlobDatabase", menuName = "Databases/Blob/BlobDatabase")]
	public class BlobDatabase : ScriptableObject
	{
		[SerializeField] private List<BlobRequirements> m_allBlobs = new List<BlobRequirements>();

		public IReadOnlyList<BlobRequirements> AllBlobConfigurations => m_allBlobs;

		public BlobRequirements GetBlobByTileType (TileType type)
		{
			var blob = m_allBlobs.FirstOrDefault(b => b.RegionType == type);
			return blob;
		}

#if UNITY_EDITOR
		[ContextMenu("Refresh Database")]
		public void Refresh ()
		{
			string[] guids = AssetDatabase.FindAssets("t:BlobRequirements");
			m_allBlobs = guids
				.Select(guid => AssetDatabase.LoadAssetAtPath<BlobRequirements>(AssetDatabase.GUIDToAssetPath(guid)))
				.Where(t => t != null)
				.ToList();

			EditorUtility.SetDirty(this);
			Debug.Log($"BlobDatabase refreshed. Found {m_allBlobs.Count} BlobRequirements.");
		}
#endif
	}
}
