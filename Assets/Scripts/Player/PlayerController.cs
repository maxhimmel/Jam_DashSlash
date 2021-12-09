using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Xam.Utility;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player
{
	using Weapons;
	using Animation;
	using Movement;
	using Slicing;
	using Scoring;

	public partial class PlayerController : MonoBehaviour, 
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

		[Header( "Debug" )]
		[SerializeField] private DebugCheats m_cheats = new DebugCheats();

		private LerpMotor m_motor;
		private LazyCachedComponent<ITrajectoryController> m_trajectoryController = new LazyCachedComponent<ITrajectoryController>();
		private AnimController m_animator;
		private IDamageable m_damageable;

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

		private void OnSwordBlocked( object sender, ISliceable e )
		{
			Transform sliceableObj = e.ToTransform();

			Debug.Assert( sliceableObj != null, $"Expecting ISliceable to be implemented on a Component.\n" +
				$"<b>Cannot handle this use case. Please refactor.</b>", this );

			m_damageable.TakeDamage( new IgnoreSlicingDamageDatum()
			{
				Instigator = sliceableObj,
				DamageCauser = sliceableObj
			} );
		}

		private void Update()
		{
			TryForceUpdateTrajectory();

			m_cheats.Update( this );
		}

		private void TryForceUpdateTrajectory()
		{
			if ( CanUpdateTrajectory() )
			{
				m_trajectoryController[this].ForceUpdate();
			}
		}

		private bool CanUpdateTrajectory()
		{
			return m_motor.IsMoving && m_trajectoryController[this].IsDragging;
		}

		private void Start()
		{
			m_trajectoryController[this].DragStarted += OnDragStarted;
			m_trajectoryController[this].DragReleased += OnDragReleased;
			m_trajectoryController[this].ZipUpCompleted += OnZipUpCompleted;

			m_sword.Blocked += OnSwordBlocked;
		}

		private void OnDestroy()
		{
			m_trajectoryController[this].DragStarted -= OnDragStarted;
			m_trajectoryController[this].DragReleased -= OnDragReleased;
			m_trajectoryController[this].ZipUpCompleted -= OnZipUpCompleted;

			m_sword.Blocked -= OnSwordBlocked;
		}

		private void Awake()
		{
			m_motor = GetComponentInChildren<LerpMotor>();
			m_animator = GetComponentInChildren<AnimController>();
			m_damageable = GetComponentInChildren<IDamageable>();
		}
	}
}
