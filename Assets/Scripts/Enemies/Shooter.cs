using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;

namespace DashSlash.Gameplay.Enemies
{
    using Weapons;
	using Xam.Utility.Extensions;

	public class Shooter : Enemy
    {
		[Header( "Shooter" )]
		[SerializeField, Min( 0 )] private float m_senseDangerRadius = 6;
		[SerializeField, Min( 0 )] private float m_teleportCooldown = 3;
		[SerializeField, Min( 0 )] private float m_teleportTravelDuration = 0.5f;

		[Header( "VFX" )]
		[SerializeField] private TrailRenderer m_teleportTrail = default;

		private LazyCachedChildComponent<Gun> m_gun = new LazyCachedChildComponent<Gun>( false );
		private float m_nextTeleportTime;
		private Coroutine m_teleportTravelRoutine;

		protected override void UpdateState()
		{
			base.UpdateState();

			if ( !CanTeleport() ) { return; }

			if ( CanSenseDanger() )
			{
				Teleport();
			}
		}

		private bool CanTeleport()
		{
			return m_nextTeleportTime <= Time.timeSinceLevelLoad;
		}

		private bool CanSenseDanger()
		{
			float distSqrToPlayer = GetDistanceSqrToPlayer();
			return distSqrToPlayer <= m_senseDangerRadius * m_senseDangerRadius;
		}

		private void Teleport()
		{
			float totalTeleportDuration = m_teleportCooldown + m_teleportTravelDuration;
			m_nextTeleportTime = Time.timeSinceLevelLoad + totalTeleportDuration;

			var playerPos = GetPlayerPosition();
			var teleportPos = playerPos + GetDirectionToPlayer() * Random.Range( 3, 10 );
			// TODO: Clamp teleportPos within Arena bounds.

			this.TryStopCoroutine( ref m_teleportTravelRoutine );
			m_teleportTravelRoutine = StartCoroutine( TeleportTravel( teleportPos ) );
		}

		private IEnumerator TeleportTravel( Vector3 teleportPos )
		{
			OnTeleportStarted();

			Vector3 startPos = Position;
			float timer = m_teleportTravelDuration > 0 ? 0 : 1;

			while ( timer < 1 )
			{
				timer += Time.deltaTime / m_teleportTravelDuration;
				timer = Mathf.Min( timer, 1 );

				Vector3 travelPos = Vector3.Lerp( startPos, teleportPos, timer );
				m_body.MovePosition( travelPos );

				yield return new WaitForFixedUpdate();
			}

			m_body.MovePosition( teleportPos );
			m_body.SetRotation( GetFacingRotationToPlayer() );

			OnTeleportComplete();

			m_teleportTravelRoutine = null;
		}

		private void OnTeleportStarted()
		{
			HideModel();
			SetBehaviourEnabled( false, m_hurtBoxes );

			m_teleportTrail.enabled = true;

			m_gun[this].StopFiring();
		}

		private void OnTeleportComplete()
		{
			ShowModel();
			SetBehaviourEnabled( true, m_hurtBoxes );

			m_gun[this].StartFiring();
		}

		protected override void BeginSpawning()
		{
			transform.rotation = GetFacingRotationToPlayer();

			base.BeginSpawning();
		}

		protected override void OnAwokenFromSpawn()
		{
			base.OnAwokenFromSpawn();

			m_gun[this].StartFiring();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_gun[this].StopFiring();
		}

#if UNITY_EDITOR
		[Header( "Editor / Tools" )]
		[SerializeField] private Color m_senseDangerColor = Color.red;

		protected override void DrawGizmosSelected()
		{
			base.DrawGizmosSelected();

			Gizmos.color = m_senseDangerColor;
			Gizmos.DrawWireSphere( Position, m_senseDangerRadius );
		}
#endif
	}
}
