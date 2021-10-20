using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

namespace DashSlash.Gameplay.Player.Animation
{
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
		[SerializeField] private CinemachineImpulseSource m_dashShake = default;

		private Tweener m_rotationAnim;

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

		public void PlayStunRecovery()
		{
			m_rotationAnim = ModelParent.DORotate( Vector3.zero, m_stunRecoveryDuration, m_stunRecoveryRotationMode )
				.SetEase( m_stunRecoveryEase );
		}

		public void ClearAllAnimations( bool complete = false )
		{
			if ( m_rotationAnim.IsActive() )
			{
				m_rotationAnim.Kill( complete );
			}
		}
	}
}
