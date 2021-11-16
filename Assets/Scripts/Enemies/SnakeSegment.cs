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

		[SerializeField] private float m_radius = 0.5f;

		private SnakeSegment m_nextSegment;
		private Rigidbody2D m_body;

		public void SetNextSegment( SnakeSegment next )
		{
			m_nextSegment = next;
		}

		public void UpdateFollowMovement()
		{
			Vector3 myPos = m_body.position;
			Vector3 nextPos = m_nextSegment.m_body.position;

			Vector3 segmentToNext = nextPos - myPos;
			Vector3 segmentPos = nextPos - segmentToNext.normalized * OffsetDistance;
			Quaternion segmentRot = Quaternion.LookRotation( Vector3.forward, segmentToNext );

			m_body.MovePosition( segmentPos );
			m_body.SetRotation( segmentRot );
		}

		private void Awake()
		{
			m_body = GetComponentInChildren<Rigidbody2D>();
			Sliceable = GetComponentInChildren<ISliceable>();
		}
	}
}
