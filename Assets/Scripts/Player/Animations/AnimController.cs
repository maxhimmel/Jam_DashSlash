using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Xam.Utility.Juicy;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player.Animation
{
	using Vfx.Audiences;

	public class AnimController : MonoBehaviour
    {
		private Transform ModelParent => Model.parent;
		private Transform Model => transform;

		[Header( "Prepare Dash" )]
		[SerializeField] private float m_punchStrength = 25;
		[SerializeField] private int m_punchVibrato = 5;
		[SerializeField] private float m_punchElasticity = 1;

		[Header( "Stun" )]
		[SerializeField] private float m_stunRecoveryDuration = 0.5f;
		[SerializeField] private RotateMode m_stunRecoveryRotationMode = RotateMode.LocalAxisAdd;
		[SerializeField] private Ease m_stunRecoveryEase = Ease.InOutBounce;

		[Header( "VFX" )]
		[SerializeField] private ParticleSystem m_dashVfx = default;
		[SerializeField] private ParticleSystem m_collectPickupVfx = default;
		[SerializeField] private CinemachineImpulseSource m_dashShake = default;
		[SerializeField] private RendererBlinker m_stunBlinker = default;
		[SerializeField] private PlayerGooglyEyesController m_googlyEyes = default;

		private Tweener m_rotationAnim;
		private Coroutine m_stunBlinkRoutine;

		public void PlayPrepareDashAnim( float duration, Vector3 trajectory )
		{
			if ( m_rotationAnim.IsActive() )
			{
				m_rotationAnim.Kill( true );
			}
			m_rotationAnim = Model.DOPunchRotation( Vector3.forward * m_punchStrength, duration, m_punchVibrato, m_punchElasticity );
		}

		public void PlayDashVfx( float duration, Vector3 trajectory )
		{
			float dashDistance = trajectory.magnitude;

			ParticleSystem.MainModule dashModule = m_dashVfx.main;
			dashModule.startSize = dashDistance * 2f;
			dashModule.startLifetime = duration;
			m_dashVfx.Play( true );

			m_dashShake.GenerateImpulseAt( Model.position, trajectory );
		}

		public void PlayStunnedVfx( float vfxDuration, float totalStunDuration )
		{
			m_stunBlinker.Play();

			this.TryStopCoroutine( ref m_stunBlinkRoutine );
			m_stunBlinkRoutine = this.StartWaitingForSeconds( vfxDuration, m_stunBlinker.Stop, true );

			m_googlyEyes.PlayDazedAnim( totalStunDuration );

			AudienceReactionFactory.Instance.PlayScaredReaction( Model.position, Vector3.up );
		}

		public void PlayStunRecovery()
		{
			m_rotationAnim = ModelParent.DORotate( Vector3.zero, m_stunRecoveryDuration, m_stunRecoveryRotationMode )
				.SetEase( m_stunRecoveryEase );
		}

		public void PlayCollectPickupVfx()
		{
			m_collectPickupVfx.Emit( 1 );
		}

		public void ClearAllAnimations()
		{
			if ( m_rotationAnim.IsActive() )
			{
				m_rotationAnim.Kill( true );
			}

			m_stunBlinker.Stop( true );
			this.TryStopCoroutine( ref m_stunBlinkRoutine );
		}
	}
}
