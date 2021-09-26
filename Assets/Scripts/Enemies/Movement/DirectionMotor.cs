using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Movement
{
    public class DirectionMotor : MonoBehaviour
    {
		[SerializeField] private float m_maxSpeed = 4;
		[SerializeField] private float m_acceleration = 10;

		private Rigidbody2D m_body;
		private Vector3 m_velocity;
		private Vector3 m_desiredVelocity;

		public void SetDesiredVelocity( Vector3 direction )
		{
			m_desiredVelocity = direction * m_maxSpeed;
		}

		private void FixedUpdate()
		{
			UpdateState();
			Accelerate();
			ApplyMovement();
		}

		private void UpdateState()
		{
			m_velocity = m_body.velocity;
		}

		private void Accelerate()
		{
			float speedDelta = m_acceleration * Time.deltaTime;
			m_velocity = Vector3.MoveTowards( m_velocity, m_desiredVelocity, speedDelta );
		}

		private void ApplyMovement()
		{
			m_body.velocity = m_velocity;
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
