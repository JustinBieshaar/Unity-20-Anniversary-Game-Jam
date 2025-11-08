using Assets._Code.Building.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Code.Blobs.Data
{
	[CreateAssetMenu(fileName = "BlobRequirement", menuName = "Databases/Blob/BlobRequirement")]
	public class BlobRequirements : ScriptableObject
	{
		[SerializeField] private TileType m_regionType;
		[SerializeField] private Color m_color;
		[SerializeField] private Blob m_blobPrefab;

		[Header("Requirements")]
		[SerializeField] private int m_minTiles;
		[SerializeField] private int m_tilesPerBlob;
		[SerializeField] private int m_minTrees;
		[SerializeField] private int m_maxTrees;

		public TileType RegionType => m_regionType;
		public Color Color => m_color;
		public Blob BlobPrefab => m_blobPrefab;

		public int TileRequirement => m_minTiles;
		public int TileRequirementPerBlob => m_tilesPerBlob;
		public int MinTreeRequirement => m_minTrees;
		public int MaxTreeRequirement => m_maxTrees;
	}
}
