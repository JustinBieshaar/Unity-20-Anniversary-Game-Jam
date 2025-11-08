using Assets._Code.Blobs.Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Assets._Code.Blobs.BlobManager;

namespace Assets._Code.Blobs
{
	public class Blob : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer m_renderer;
		[SerializeField] private Animator m_animator;

		[Header("Movement")]
		[SerializeField] private float m_moveSpeed = 2f;
		[SerializeField] private Vector2 m_idleTimeRange = new Vector2(1.5f, 4f);

		private GroundRegion m_region;

		public void Init (BlobRequirements blob, GroundRegion region)
		{
			m_renderer.color = blob.Color;
			m_region = region;
			StartCoroutine(RoamRoutine());
		}

		private IEnumerator RoamRoutine ()
		{
			var tiles = m_region.Tiles;
			var neighbors = m_region.Neighbors;
			var world = m_region.WorldPositions;
			if (tiles.Count == 0) yield break;

			Vector3Int current = tiles[Random.Range(0, tiles.Count)];
			transform.position = world[current];

			Vector3 baseScale = transform.localScale;
			while (true)
			{
				yield return new WaitForSeconds(Random.Range(m_idleTimeRange.x, m_idleTimeRange.y));

				if (!neighbors.ContainsKey(current) || neighbors[current].Count == 0)
					continue;

				Vector3Int next = neighbors[current][Random.Range(0, neighbors[current].Count)];
				Vector3 target = world[next];

				// DOTween sequence for squash → move → stretch
				Sequence seq = DOTween.Sequence();

				// Squash before moving
				seq.Append(transform.DOScale(new Vector3(baseScale.x * 0.8f, baseScale.y / 0.8f, baseScale.z), 0.1f).SetEase(Ease.OutQuad));

				// Move while stretching slightly
				seq.Append(transform.DOMove(target, Vector3.Distance(transform.position, target) / m_moveSpeed)
					.SetEase(Ease.Linear)
					.OnUpdate(() =>
					{
						float t = Mathf.PingPong(Time.time * 6f, 0.05f);
						transform.localScale = baseScale + new Vector3(-t, t, 0f);
					}));

				// Stretch after landing
				seq.Append(transform.DOScale(baseScale * 1.2f, 0.15f).SetEase(Ease.OutBack));
				seq.Append(transform.DOScale(baseScale, 0.1f).SetEase(Ease.OutBack));

				// Wait for the sequence to finish
				yield return seq.WaitForCompletion();

				current = next;
			}
		}
	}
}
