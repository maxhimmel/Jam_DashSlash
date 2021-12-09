using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;
using DG.Tweening;

namespace DashSlash.Gameplay.Player
{
	using Movement;
	using Animation;
	using Weapons;
	using Scoring;

	public class DamageHandler : MonoBehaviour, IDamageable
	{
		public bool IsStunned => m_stunRoutine != null;

		private ScoreController Score => ScoreController.Instance;

		[Header( "Stunned" )]
		[SerializeField] private float m_stunDuration = 1;
		[SerializeField] private float m_stunVfxDuration = 0.5f;

		[Header( "Physics" )]
		[SerializeField] private float m_knockbackForce = 8;
		[SerializeField] private float m_knockbackTorque = 8;

		[Header( "Trajectory" )]
		[SerializeField] private float m_retrieveReticleDuration = 0.5f;
		[SerializeField] private Ease m_retrieveReticleAnim = Ease.InOutBack;

		[Header( "Attacking" )]
		[SerializeField] private Sword m_sword = default;
		[SerializeField] private Sword m_recoverySword = default;

		private LerpMotor m_motor;
		private Rigidbody2D m_body;
		private AnimController m_animController;
		private ITrajectoryController m_trajectoryController;
		private Coroutine m_stunRoutine;
		private Coroutine m_recoverySliceRoutine;

		public void TakeDamage( DamageDatum dmgData )
		{
			if ( !CanTakeDamage( dmgData ) ) { return; }

			PrepareForDamage( dmgData );

			ApplyDamage( dmgData );

			m_stunRoutine = StartCoroutine( UpdateStunState() );
		}

		private bool CanTakeDamage( DamageDatum dmgData )
		{
			if ( IsStunned ) { return false; }
			if ( !m_sword.IsSlicing || dmgData is IgnoreSlicingDamageDatum ) { return true; }

			Vector3 dmgDir = dmgData.GetHitDirection( transform.position );
			float hitDot = Vector3.Dot( dmgDir, transform.up );

			return hitDot <= 0;
		}

		private void PrepareForDamage( DamageDatum dmgData )
		{
			m_sword.StopSlicing( true );

			m_trajectoryController.SetActive( false );
			m_trajectoryController.RetrieveReticle( m_retrieveReticleDuration, m_retrieveReticleAnim );

			m_motor.ClearMovement();
			m_body.freezeRotation = false;

			m_animController.ClearAllAnimations();
			m_animController.PlayStunnedVfx( m_stunVfxDuration, m_stunDuration );

			Score.ForceClearCombo();
			Score.SetPickupScoringActive( false );
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
			m_trajectoryController.SetActive( true );

			m_body.freezeRotation = true;
			m_body.angularVelocity = 0;

			m_animController.PlayStunRecovery();

			this.TryStopCoroutine( ref m_recoverySliceRoutine );
			m_recoverySliceRoutine = StartCoroutine( PerformRecoverySlice() );

			Score.SetPickupScoringActive( true );
		}

		private IEnumerator PerformRecoverySlice()
		{
			m_recoverySword.StartSlicing();
			yield return null;
			m_recoverySword.StopSlicing( false );

			m_recoverySliceRoutine = null;
		}

		private void Awake()
		{
			m_motor = GetComponent<LerpMotor>();
			m_body = GetComponent<Rigidbody2D>();
			m_animController = GetComponentInChildren<AnimController>();
			m_trajectoryController = GetComponent<ITrajectoryController>();
		}
	}
}
