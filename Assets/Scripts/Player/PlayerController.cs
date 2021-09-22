using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DashSlash.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
	{
		[Header( "Movement" )]
		[SerializeField] private Ease m_startEase = Ease.OutQuad;
		[SerializeField] private float m_startMoveDuration = 0.2f;
		[SerializeField] private Ease m_dashEase = Ease.OutQuad;
		[SerializeField] private float m_dashMoveDuration = 0.4f;

		private LerpMotor m_motor;
		private PlayerTrajectoryController m_trajectoryController;

		private void OnDragStarted( object sender, DragArgs e )
		{
			Vector3 moveDir = e.Start - m_motor.Position;

			m_motor.SetEase( m_startEase );
			m_motor.SetDuration( m_startMoveDuration );
			m_motor.SetDesiredVelocity( moveDir );
		}

		private void OnDragReleased( object sender, DragArgs e )
		{
			Vector3 moveDir = e.End - m_motor.Position;

			m_motor.SetEase( m_dashEase );
			m_motor.SetDuration( m_dashMoveDuration );
			m_motor.SetDesiredVelocity( moveDir );
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
