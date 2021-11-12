using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player
{
	using Weapons;
	using Animation;
	using Movement;

	public class PlayerController : MonoBehaviour, 
		ICollector
	{
		private ScoreController Score => ScoreController.Instance;

		[Header( "Movement" )]
		[SerializeField] private Ease m_prepareEase = Ease.OutQuint;
		[SerializeField] private float m_prepareMoveDuration = 0.2f;
		[SerializeField] private Ease m_dashEase = Ease.OutCirc;
		[SerializeField] private float m_dashMoveDuration = 0.4f;

		[Header( "Attacking" )]
		[SerializeField] private Sword m_sword = default;

		private LerpMotor m_motor;
		private PlayerTrajectoryController m_trajectoryController;
		private AnimController m_animator;

		void ICollector.Collect( Pickup pickup )
		{
			m_animator.PlayCollectPickupVfx();
			Score.AddPickup();
		}

		private void OnDragStarted( object sender, DragArgs e )
		{
			Vector3 moveDir = e.Start - m_motor.Position;

			m_sword.StopSlicing( false );

			m_motor.SetEase( m_prepareEase );
			m_motor.SetDuration( m_prepareMoveDuration );
			m_motor.SetDesiredVelocity( moveDir );

			m_animator.PlayPrepareDashAnim( m_prepareMoveDuration, moveDir );
			m_animator.PlayDashVfx( m_prepareMoveDuration, moveDir );
		}

		private void OnDragReleased( object sender, DragArgs e )
		{
			Vector3 moveDir = e.End - m_motor.Position;

			m_sword.SetRotation( e.Vector );
			m_sword.StartSlicing();

			m_motor.SetEase( m_dashEase );
			m_motor.SetDuration( m_dashMoveDuration );
			m_motor.SetDesiredVelocity( moveDir );

			m_animator.PlayDashVfx( m_dashMoveDuration, moveDir );

			Score.BeginCombo();
		}

		private void OnZipUpCompleted( object sender, DragArgs e )
		{
			m_sword.StopSlicing( true );
			Score.TryClearBonus();
		}

		private void OnSwordSliced( object sender, System.EventArgs e )
		{
			Score.AddSliceKill();
		}

		[Header( "Editor / Debug" )]
		[SerializeField] private int m_pickupColections = 10;

		private void Update()
		{
			TryForceUpdateTrajectory();

			if ( Input.GetKeyDown( KeyCode.Space ) )
			{
				ICollector self = this;
				for ( int idx = 0; idx < m_pickupColections; ++idx )
				{
					self.Collect( null );
				}
			}
			if ( Input.GetKeyDown( KeyCode.Return ) )
			{
				IDamageable damaegable = GetComponentInChildren<IDamageable>();
				damaegable.TakeDamage( new DamageDatum()
				{
					DamageCauser = transform,
					Instigator = transform
				} );
			}
			if ( Input.GetKeyDown( KeyCode.Backspace ) )
			{
				Vfx.Audiences.AudienceReactionFactory.Instance.PlayExcitedReaction( transform.position, Vector3.up );
			}
		}

		private void TryForceUpdateTrajectory()
		{
			if ( CanUpdateTrajectory() )
			{
				m_trajectoryController.ForceUpdate();
			}
		}

		private bool CanUpdateTrajectory()
		{
			return m_motor.IsMoving && m_trajectoryController.IsDragging;
		}

		private void Start()
		{
			m_trajectoryController.DragStarted += OnDragStarted;
			m_trajectoryController.DragReleased += OnDragReleased;
			m_trajectoryController.ZipUpCompleted += OnZipUpCompleted;

			m_sword.Sliced += OnSwordSliced;
		}

		private void OnDestroy()
		{
			m_trajectoryController.DragStarted -= OnDragStarted;
			m_trajectoryController.DragReleased -= OnDragReleased;
			m_trajectoryController.ZipUpCompleted -= OnZipUpCompleted;

			m_sword.Sliced -= OnSwordSliced;
		}

		private void Awake()
		{
			m_motor = GetComponentInChildren<LerpMotor>();
			m_trajectoryController = GetComponent<PlayerTrajectoryController>();
			m_animator = GetComponentInChildren<AnimController>();
		}
	}
}
