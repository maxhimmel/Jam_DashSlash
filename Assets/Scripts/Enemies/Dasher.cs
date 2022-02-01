using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Juicy;
using Xam.Utility.Randomization;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Enemies
{
    using Movement;

	public class Dasher : Enemy
    {
		[Header( "Dasher" )]
		[SerializeField] private float m_dashOffsetAngle = 15f;
		[SerializeField] private float m_dashDistance = 5;

		[Space]
		[SerializeField] private float m_dashAnticDuration = 0.5f;
		[SerializeField] private float m_dashCooldownDuration = 1;

		private LerpMotor m_motor;
		private float m_nextDashTime;
		private int m_angleSign = 1;
		private ColorBlinker m_dashAnticBlinker;
		private Coroutine m_dashAnticRoutine;

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
			UpdateDashAntic();

			m_nextDashTime = Time.timeSinceLevelLoad + m_dashCooldownDuration;
			m_angleSign *= -1;
		}

		private void UpdateDashAntic()
		{
			m_dashAnticBlinker.Stop();

			float duration = m_dashCooldownDuration - m_dashAnticDuration;

			this.TryStopCoroutine( ref m_dashAnticRoutine );
			m_dashAnticRoutine = this.StartWaitingForSeconds( duration, m_dashAnticBlinker.Play );
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

			m_angleSign = RandomUtility.Sign();
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_motor = GetComponent<LerpMotor>();
			m_dashAnticBlinker = GetComponentInChildren<ColorBlinker>();
		}
	}
}
