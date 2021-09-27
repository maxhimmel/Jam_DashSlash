using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
    using Movement;

    public class Dasher : Enemy
    {
		[Header( "Dasher" )]
		[SerializeField] private float m_dashOffsetAngle = 15f;
		[SerializeField] private float m_dashCooldownDuration = 1;
		[SerializeField] private float m_dashDistance = 5;

		private LerpMotor m_motor;
		private float m_nextDashTime;
		private int m_angleSign = 1;

		protected override void UpdateState()
		{
			base.UpdateState();

			if ( CanDash() )
			{
				UpdateDashState();
				ApplyMovement();
			}
		}

		private bool CanDash()
		{
			return m_nextDashTime < Time.timeSinceLevelLoad;
		}

		private void UpdateDashState()
		{
			m_nextDashTime = Time.timeSinceLevelLoad + m_dashCooldownDuration;
			m_angleSign *= -1;
		}

		private void ApplyMovement()
		{
			Vector3 dashVelocity = GetDashDirection() * m_dashDistance;
			m_motor.SetDesiredVelocity( dashVelocity );
		}

		private Vector3 GetDashDirection()
		{
			Vector3 dashDirection = GetDirectionToPlayer();

			if ( !IsPlayerWithinDashRange() )
			{
				float dashAngle = m_dashOffsetAngle * m_angleSign;
				dashDirection = Quaternion.AngleAxis( dashAngle, Vector3.forward ) * dashDirection;
			}

			return dashDirection;
		}

		private bool IsPlayerWithinDashRange()
		{
			float distSqrToPlayer = GetDistanceSqrToPlayer();
			return distSqrToPlayer < m_dashDistance * m_dashDistance;
		}

		protected override void InitReferences()
		{
			base.InitReferences();

			m_angleSign = (Random.value >= 0.5f) ? 1 : -1;
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_motor = GetComponent<LerpMotor>();
		}
	}
}
