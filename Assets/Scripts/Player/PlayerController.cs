using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DashSlash.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
	{
		[Header( "Movement" )]
		[SerializeField] private float m_startMoveDuration = 0.2f;
		[SerializeField] private float m_dashMoveDuration = 0.4f;

		[Header( "Animation" )]
		[SerializeField] private Transform m_model = default;

		[Space]
		[SerializeField] private Ease m_startEase = Ease.OutQuad;
		[SerializeField] private Ease m_dashEase = Ease.OutQuad;

		[Space]
		[SerializeField] private float m_punchStrength = 25;
		[SerializeField] private int m_punchVibrato = 5;
		[SerializeField] private float m_punchElasticity = 1;

		[Header( "VFX" )]
		[SerializeField] private ParticleSystem m_dashVfx = default;

		private LerpMotor m_motor;
		private PlayerTrajectoryController m_trajectoryController;
		private Tweener m_rotationAnim;

		private void OnDragStarted( object sender, DragArgs e )
		{
			Vector3 moveDir = e.Start - m_motor.Position;

			m_motor.SetEase( m_startEase );
			m_motor.SetDuration( m_startMoveDuration );
			m_motor.SetDesiredVelocity( moveDir );

			if ( m_rotationAnim.IsActive() )
			{
				m_rotationAnim.Kill( true );
			}
			m_rotationAnim = m_model.DOPunchRotation( Vector3.forward * m_punchStrength, m_startMoveDuration, m_punchVibrato, m_punchElasticity );

			PlayDashVfx( m_startMoveDuration, moveDir.magnitude );
		}

		private void OnDragReleased( object sender, DragArgs e )
		{
			Vector3 moveDir = e.End - m_motor.Position;

			m_motor.SetEase( m_dashEase );
			m_motor.SetDuration( m_dashMoveDuration );
			m_motor.SetDesiredVelocity( moveDir );

			PlayDashVfx( m_dashMoveDuration, moveDir.magnitude );
		}

		private void PlayDashVfx( float duration, float radius )
		{
			ParticleSystem.MainModule dashModule = m_dashVfx.main;
			dashModule.startSize = radius * 2f;
			dashModule.startLifetime = duration;

			m_dashVfx.Play( true );
		}

		private void Update()
		{
			TryForceUpdateTrajectory();
		}

		private void TryForceUpdateTrajectory()
		{
			if ( m_motor.IsMoving && m_trajectoryController.IsDragging )
			{
				m_trajectoryController.ForceUpdate();
			}
		}

		private void Start()
		{
			m_trajectoryController.DragStarted += OnDragStarted;
			m_trajectoryController.DragReleased += OnDragReleased;
		}

		private void OnDestroy()
		{
			m_trajectoryController.DragStarted -= OnDragStarted;
			m_trajectoryController.DragReleased -= OnDragReleased;
		}

		private void Awake()
		{
			m_motor = GetComponentInChildren<LerpMotor>();
			m_trajectoryController = GetComponent<PlayerTrajectoryController>();
		}
	}
}
