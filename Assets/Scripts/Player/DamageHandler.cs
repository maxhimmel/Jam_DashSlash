using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;
using DG.Tweening;

namespace DashSlash.Gameplay.Player
{
	using Movement;
	using Animation;

	public class DamageHandler : MonoBehaviour, IDamageable
	{
		public bool IsStunned => m_stunRoutine != null;

		[Header( "Stunned" )]
		[SerializeField] private float m_stunDuration = 1;
		[SerializeField] private float m_stunVfxDuration = 0.5f;

		[Header( "Physics" )]
		[SerializeField] private float m_knockbackForce = 8;
		[SerializeField] private float m_knockbackTorque = 8;

		[Header( "Trajectory" )]
		[SerializeField] private float m_retrieveReticleDuration = 0.5f;
		[SerializeField] private Ease m_retrieveReticleAnim = Ease.InOutBack;

		private LerpMotor m_motor;
		private Rigidbody2D m_body;
		private AnimController m_animController;
		private PlayerTrajectoryController m_trajectoryController;
		private Coroutine m_stunRoutine;

		public void TakeDamage( DamageDatum dmgData )
		{
			if ( IsStunned ) { return; }

			PrepareForDamage( dmgData );

			ApplyDamage( dmgData );

			m_stunRoutine = StartCoroutine( UpdateStunState() );
		}

		private void PrepareForDamage( DamageDatum dmgData )
		{
			m_trajectoryController.enabled = false;
			m_trajectoryController.RetrieveReticle( m_retrieveReticleDuration, m_retrieveReticleAnim );

			m_motor.ClearMovement();
			m_body.freezeRotation = false;

			m_animController.ClearAllAnimations( false );
			m_animController.PlayStunnedVfx( m_stunVfxDuration );
		}

		private void ApplyDamage( DamageDatum dmgData )
		{
			Vector3 hitDir = dmgData.GetHitDirection( transform.position );
			m_body.AddForce( hitDir * m_knockbackForce, ForceMode2D.Impulse );
			m_body.AddTorque( m_knockbackTorque, ForceMode2D.Impulse );
		}

		private IEnumerator UpdateStunState()
		{
			yield return new WaitForSeconds( m_stunDuration );

			m_stunRoutine = null;

			OnRecovered();
		}

		private void OnRecovered()
		{
			m_trajectoryController.enabled = true;

			m_body.freezeRotation = true;
			m_body.angularVelocity = 0;

			m_animController.PlayStunRecovery();
		}

		private void Awake()
		{
			m_motor = GetComponent<LerpMotor>();
			m_body = GetComponent<Rigidbody2D>();
			m_animController = GetComponentInChildren<AnimController>();
			m_trajectoryController = GetComponent<PlayerTrajectoryController>();
		}
	}
}
