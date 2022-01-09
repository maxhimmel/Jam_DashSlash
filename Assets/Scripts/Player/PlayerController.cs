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
	using Slicing;
	using Scoring;

	public partial class PlayerController : MonoBehaviour, 
		ICollector, ITrajectoryControllerContainer
	{
		public bool IsAttacking => m_sword.IsSlicing;
		public Vector3 Velocity => m_motor.Velocity;

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

		private IInterpolationMovement m_motor;
		private AnimController m_animator;
		private IDamageable m_damageable;
		private ITrajectoryController m_trajectoryController;
		//private List<Vector3> m_dragPoints = new List<Vector3>();

		void ICollector.Collect( Pickup pickup )
		{
			m_animator.PlayCollectPickupVfx();
			Score.AddPickup();
		}

		void ITrajectoryControllerContainer.InstallTrajectory( ITrajectoryController controller )
		{
			if ( m_trajectoryController != null )
			{
				m_trajectoryController.DragStarted -= OnDragStarted;
				//m_trajectoryController.DragUpdated -= OnDragUpdated;
				m_trajectoryController.DragReleased -= OnDragReleased;
				m_trajectoryController.ZipUpCompleted -= OnZipUpCompleted;
			}

			if ( controller != null )
			{
				controller.DragStarted += OnDragStarted;
				//controller.DragUpdated += OnDragUpdated;
				controller.DragReleased += OnDragReleased;
				controller.ZipUpCompleted += OnZipUpCompleted;
			}

			m_trajectoryController = controller;
		}

		private void OnDragStarted( object sender, DragArgs e )
		{
			//m_dragPoints.Clear();
			//m_dragPoints.Add( e.Start );

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
			//m_dragPoints.Add( e.End );

			//var nearestZero = Mathf.Infinity;
			//var mostAlignedPoint = Vector3.zero;
			//var midpoint = (e.Start + e.End) / 2f;
			//var dragDirection = e.Vector;

			//for ( int idx = 1; idx < m_dragPoints.Count - 1; ++idx )
			//{
			//	var dragPos = m_dragPoints[idx];
			//	var midPointToDragPos = (dragPos - midpoint);
			//	float dot = Mathf.Abs( Vector3.Dot( midPointToDragPos, dragDirection ) );

			//	if ( dot < nearestZero )
			//	{
			//		nearestZero = dot;
			//		mostAlignedPoint = dragPos;
			//	}
			//}


			//var cross = Vector3.Cross( dragDirection, Vector3.forward );
			//var signDir = Mathf.Sign( Vector3.Dot( cross, (mostAlignedPoint - midpoint) ) );
			//float distFromMidpoint = (midpoint - mostAlignedPoint).magnitude;

			//if ( m_dragPoints.Count <= 3 )
			//{
			//	distFromMidpoint = 0;
			//}

			Vector3 moveDir = e.End - m_motor.Position;

			m_sword.SetRotation( e.Vector );
			m_sword.StartSlicing();

			m_motor.SetEase( m_dashEase );
			m_motor.SetDuration( m_dashMoveDuration );
			m_motor.SetDesiredVelocity( moveDir );//, distFromMidpoint * signDir );

			m_animator.PlayDashVfx( m_dashMoveDuration, moveDir );

			Score.BeginCombo();
		}

		//private void OnDragUpdated( object sender, DragArgs e )
		//{
		//	var prevDragPoint = m_dragPoints[m_dragPoints.Count - 1];
		//	if ( (prevDragPoint - e.End).sqrMagnitude > 0.01f )
		//	{
		//		m_dragPoints.Add( e.End );
		//	}
		//}

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
				m_trajectoryController.ForceUpdate();
			}
		}

		private bool CanUpdateTrajectory()
		{
			return m_motor.IsMoving && m_trajectoryController.IsDragging;
		}

		private void Start()
		{
			ITrajectoryControllerContainer trajectoryContainer = this;
			trajectoryContainer.InstallTrajectory( m_trajectoryController );

			m_sword.Blocked += OnSwordBlocked;
		}

		private void OnDestroy()
		{
			m_trajectoryController.DragStarted -= OnDragStarted;
			//m_trajectoryController.DragUpdated -= OnDragUpdated;
			m_trajectoryController.DragReleased -= OnDragReleased;
			m_trajectoryController.ZipUpCompleted -= OnZipUpCompleted;

			m_sword.Blocked -= OnSwordBlocked;
		}

		private void Awake()
		{
			m_motor = GetComponentInChildren<IInterpolationMovement>();
			m_animator = GetComponentInChildren<AnimController>();
			m_damageable = GetComponentInChildren<IDamageable>();
			m_trajectoryController = GetComponent<ITrajectoryController>();
		}
	}
}
