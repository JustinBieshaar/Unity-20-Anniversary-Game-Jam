using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Player
{
	public class PlayerMovementController : MonoBehaviour
	{
		[SerializeField] private Tilemap m_tileMap;
		[SerializeField] private List<TileBase> m_acceptableTiles;

		[SerializeField] private float m_speed = 5f;
		[SerializeField] private float m_friction = 5f; // Higher = stops faster

		private InputControls m_inputControls;
		private Vector3 m_direction;
		private Vector3 m_velocity;

		private void Awake ()
		{
			m_inputControls = new InputControls();
		}

		private void Start ()
		{
			m_inputControls.Player.Movement.performed += ctx => m_direction = ctx.ReadValue<Vector2>();
			m_inputControls.Player.Movement.canceled += _ => m_direction = Vector2.zero;
		}

		private void Update ()
		{
			Vector3 currentPos = transform.position;
			Vector3 isoDirection = GetIsometricDirection(m_direction);

			// Accelerate when input exists
			if (m_direction.magnitude > 0.1f)
			{
				m_velocity = isoDirection * m_speed;
			}
			else
			{
				// Apply friction (smooth deceleration)
				m_velocity = Vector3.Lerp(m_velocity, Vector3.zero, m_friction * Time.deltaTime);
			}

			// Predict next position
			Vector3 nextPos = currentPos + m_velocity * Time.deltaTime;

			// Separate X/Y checks for tile validation
			Vector3 nextPosX = new Vector3(nextPos.x, currentPos.y, currentPos.z);
			if (IsValidTile(nextPosX))
				currentPos.x = nextPos.x;

			Vector3 nextPosY = new Vector3(currentPos.x, nextPos.y, currentPos.z);
			if (IsValidTile(nextPosY))
				currentPos.y = nextPos.y;

			transform.position = currentPos;
		}

		private bool IsValidTile (Vector3 position)
		{
			Vector3Int gridPosition = m_tileMap.WorldToCell(position);
			var tile = m_tileMap.GetTile(gridPosition);
			return m_tileMap.HasTile(gridPosition) && m_acceptableTiles.Contains(tile);
		}

		private Vector3 GetIsometricDirection (Vector2 input)
		{
			Vector3 direction = new Vector3(
				input.x - input.y,
				(input.x + input.y) * 0.5f,
				0f
			);
			return direction.normalized;
		}

		private void OnEnable () => m_inputControls.Enable();
		private void OnDisable () => m_inputControls.Disable();
	}
}
