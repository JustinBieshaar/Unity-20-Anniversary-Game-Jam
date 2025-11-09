using Assets._Code.UI.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Controls
{
	[RequireComponent(typeof(Camera))]
	public class CameraController2D : MonoBehaviour
	{
		[Header("Movement")]
		[SerializeField] private float m_moveSpeed = 10f;
		[SerializeField] private float m_dragSpeed = 5f;

		[Header("Zoom")]
		[SerializeField] private float m_zoomSpeed = 5f;
		[SerializeField] private float m_minZoom = 3f;
		[SerializeField] private float m_maxZoom = 15f;

		[Header("Bounds")]
		[SerializeField] private Tilemap m_boundTilemap;
		[SerializeField] private float m_extendBoundVertical = 2;
		[SerializeField] private float m_extendBoundHorizontal = 5;

		[SerializeField] private bool m_clamp;

		private Camera m_cam;
		private Vector3 m_dragOrigin;
		private bool m_dragging;

		private Vector3 m_minBounds;
		private Vector3 m_maxBounds;

		private int m_boundCount = 0;

		private void Start ()
		{
			m_cam = GetComponent<Camera>();
			if (m_boundTilemap != null)
				CalculateBounds();
		}

		private void Update ()
		{
			if (BuildSelector.Instance.IsVisible)
			{
				return;
			}

			//HandleKeyboardMovement();
			HandleMouseDrag();
			HandleZoom();

			if (m_boundTilemap != null)
			{
				TileBase[] allTiles = m_boundTilemap.GetTilesBlock(m_boundTilemap.cellBounds);
				if (m_boundCount != allTiles.Length)
				{
					m_boundCount = allTiles.Length;
					CalculateBounds();
				}
				ClampCamera();
			}
		}

		private void HandleKeyboardMovement ()
		{
			Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
			if (input.sqrMagnitude > 0.01f)
			{
				transform.position += input.normalized * m_moveSpeed * Time.deltaTime;
			}
		}

		private void HandleMouseDrag ()
		{
			if (Input.GetMouseButtonDown(2)) // Middle mouse drag
			{
				m_dragOrigin = m_cam.ScreenToWorldPoint(Input.mousePosition);
				m_dragging = true;
			}

			if (Input.GetMouseButtonUp(2))
			{
				m_dragging = false;
			}

			if (m_dragging)
			{
				Vector3 difference = m_dragOrigin - m_cam.ScreenToWorldPoint(Input.mousePosition);
				transform.position += difference;
			}
		}

		private void HandleZoom ()
		{
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (Mathf.Abs(scroll) > 0.01f)
			{
				m_cam.orthographicSize -= scroll * m_zoomSpeed;
				m_cam.orthographicSize = Mathf.Clamp(m_cam.orthographicSize, m_minZoom, m_maxZoom);
				if (m_boundTilemap != null)
					ClampCamera();
			}
		}

		private void ClampCamera ()
		{
			float vertExtent = m_cam.orthographicSize;
			float horzExtent = vertExtent * m_cam.aspect;

			vertExtent -= m_extendBoundVertical;
			horzExtent -= m_extendBoundHorizontal;

			if (vertExtent < 0) vertExtent *= -1;
			if (horzExtent < 0) horzExtent *= -1;

			float minX = m_minBounds.x - horzExtent;
			float maxX = m_maxBounds.x + horzExtent;
			float minY = m_minBounds.y - vertExtent;
			float maxY = m_maxBounds.y + vertExtent;

			if (!m_clamp) return;
			Vector3 pos = transform.position;
			pos.x = Mathf.Clamp(pos.x, minX, maxX);
			pos.y = Mathf.Clamp(pos.y, minY, maxY);
			transform.position = pos;
		}

		private void CalculateBounds ()
		{
			// Convert tilemap bounds to world-space corners
			BoundsInt cellBounds = m_boundTilemap.cellBounds;

			Vector3Int minCell = cellBounds.min;
			Vector3Int maxCell = cellBounds.max;

			Vector3 minWorld = m_boundTilemap.CellToWorld(minCell);
			Vector3 maxWorld = m_boundTilemap.CellToWorld(maxCell);

			// Expand a bit so camera can see edge tiles
			m_minBounds = minWorld;
			m_maxBounds = maxWorld + m_boundTilemap.cellSize;
		}

#if UNITY_EDITOR
		private void OnDrawGizmosSelected ()
		{
			if (m_boundTilemap == null) return;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube((m_minBounds + m_maxBounds) / 2f, m_maxBounds - m_minBounds);
		}
#endif
	}
}
