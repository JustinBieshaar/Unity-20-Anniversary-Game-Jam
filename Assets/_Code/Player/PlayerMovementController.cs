using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets._Code.Player
{
	public class PlayerMovementController : MonoBehaviour
	{
		[SerializeField] private Tilemap m_tileMap;
		[SerializeField] private List<TileBase> m_acceptableTiles;

		[SerializeField] private float m_speed = 5;

		private InputControls m_inputControls;

		private Vector3 m_direction;

		private Vector3 NextPosition => transform.position += m_direction * m_speed;

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
			if (m_direction.magnitude <= 0) return;

			Vector3 isoDirection = GetIsometricDirection(m_direction);
			Vector3 nextPos = transform.position + isoDirection * m_speed * Time.deltaTime;

			if (IsValidTile(nextPos))
			{
				transform.position = nextPos;
			}
			else
			{
				m_direction = Vector2.zero;
			}
		}

		private bool IsValidTile (Vector3 position)
		{
			Vector3Int gridPosition = m_tileMap.WorldToCell(position);
			var tile = m_tileMap.GetTile(gridPosition);

			return m_tileMap.HasTile(gridPosition) && m_acceptableTiles.Contains(tile);
		}

		private Vector3 GetIsometricDirection (Vector2 input)
		{
			// Maps screen input to isometric world movement
			Vector3 direction = new Vector3(input.x + input.y, input.y - input.x, 0f);
			return direction;
		}

		private void OnEnable ()
		{
			m_inputControls.Enable();
		}

		private void OnDisable ()
		{ 
			m_inputControls.Disable(); 
		}
	}
}
