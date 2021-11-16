using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
	using Slicing;

    public class SnakeSegment : MonoBehaviour
    {
		public float OffsetDistance => m_radius * 2f;
		public ISliceable Sliceable { get; private set; }

		private Vector3 FacingDirection => transform.up;
		private Vector3 DirectionToNextSegment => (m_nextSegment.m_body.position - m_body.position).normalized;

		[SerializeField] private float m_radius = 0.5f;
		[SerializeField, Range( 0, 2 )] private float m_facingCorrectionDamping = 0.15f;

		private SnakeSegment m_nextSegment;
		private Rigidbody2D m_body;
		private Vector3 m_facingCorrectionVelocity;

		public void SetNextSegment( SnakeSegment next )
		{
			m_nextSegment = next;
		}

		public void UpdateFollowMovement()
		{
			m_body.MovePosition( GetFollowPosition() );
			m_body.SetRotation( GetFollowRotation() );
		}

		private Vector3 GetFollowPosition()
		{
			Vector3 nextPos = m_nextSegment.m_body.position;
			Vector3 nextDir = m_nextSegment.FacingDirection;

			Vector3 followPos = nextPos - DirectionToNextSegment * OffsetDistance;
			Vector3 targetPos = nextPos - nextDir * OffsetDistance;

			return Vector3.SmoothDamp( followPos, targetPos, ref m_facingCorrectionVelocity, m_facingCorrectionDamping );
		}

		private Quaternion GetFollowRotation()
		{
			return Quaternion.LookRotation( Vector3.forward, DirectionToNextSegment );
		}

		private void Awake()
		{
			m_body = GetComponentInChildren<Rigidbody2D>();
			Sliceable = GetComponentInChildren<ISliceable>();
		}
	}
}
