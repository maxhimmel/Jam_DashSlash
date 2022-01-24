using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DashSlash.Gameplay.Enemies
{
    using Movement;

    public class DirectionMover : Enemy
    {
		[Header( "Dodging" )]
		[SerializeField] private bool m_dodgingEnabled = true;
		[ShowIfGroup( "Dodging", VisibleIf = "m_dodgingEnabled" )]
		[SerializeField] private float m_senseDangerRadius = 5;

		[Space]
		[ShowIfGroup( "Dodging", VisibleIf = "m_dodgingEnabled" )]
		[SerializeField] private float m_dodgeForce = 10;
		[ShowIfGroup( "Dodging", VisibleIf = "m_dodgingEnabled" )]
		[SerializeField] private float m_dodgeDrag = 50;

		[Space]
		[ShowIfGroup( "Dodging", VisibleIf = "m_dodgingEnabled" )]
		[SerializeField] private float m_dodgeCooldown = 1;
		[ShowIfGroup( "Dodging", VisibleIf = "m_dodgingEnabled" )]
		[SerializeField] private float m_dodgeDuration = 0.3f;

		protected CharacterMotor m_motor;
		private float m_initialAcceleration = -1;
		private float m_nextDodgeTime = 0;
		private float m_dodgeCooldownEndTime = 0;

		protected override void UpdateState()
		{
			base.UpdateState();

			if ( IsDodging() )
			{
				return;
			}
			else if ( CanDodge() )
			{
				Dodge();
			}
			else
			{
				m_motor.SetAcceleration( m_initialAcceleration );
				m_motor.SetDesiredVelocity( FacingDirection );
			}
		}

		private bool IsDodging()
		{
			return m_dodgingEnabled && m_nextDodgeTime > Time.timeSinceLevelLoad;
		}

		protected virtual bool CanDodge()
		{
			if ( !m_dodgingEnabled ) { return false; }

			if ( m_dodgeCooldownEndTime > Time.timeSinceLevelLoad ) { return false; }

			if ( Mathf.Approximately( m_dodgeForce, 0 ) ) { return false; }

			float distToPlayerSqr = GetDistanceSqrToPlayer();
			if ( distToPlayerSqr > m_senseDangerRadius * m_senseDangerRadius ) { return false; }

			if ( !IsPlayerAttacking() ) { return false; }

			var dirToPlayer = GetDirectionToPlayer();
			var playerVelocity = GetPlayerVelocity();
			if ( Vector3.Dot( dirToPlayer, playerVelocity ) > 0 ) { return false; }

			return true;
		}

		private void Dodge()
		{
			m_nextDodgeTime = Time.timeSinceLevelLoad + m_dodgeDuration;
			m_dodgeCooldownEndTime = Time.timeSinceLevelLoad + m_dodgeCooldown;

			m_motor.SetAcceleration( m_dodgeDrag );
			m_motor.SetDesiredVelocity( Vector3.zero );


			var dirToPlayer = GetDirectionToPlayer();
			var dodgeDir = Quaternion.AngleAxis( 90, Vector3.forward ) * dirToPlayer;

			var dodgeForce = dodgeDir * m_dodgeForce;
			m_motor.AddForce( dodgeForce );
		}

		protected override void InitReferences()
		{
			base.InitReferences();

			m_initialAcceleration = m_motor.Acceleration;
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_motor = GetComponent<CharacterMotor>();
		}

		[Header( "Editor / Tools" )]
		[ShowIfGroup( "Dodging", VisibleIf = "m_dodgingEnabled" )]
		[SerializeField] private Color m_senseDangerColor = Color.red;

		protected override void DrawGizmos()
		{
			base.DrawGizmos();

			Gizmos.color = m_senseDangerColor;

			try
			{
				Gizmos.DrawWireSphere( Position, m_senseDangerRadius );
			}
			catch
			{
				Gizmos.DrawWireSphere( transform.position, m_senseDangerRadius );
			}
		}
	}
}
