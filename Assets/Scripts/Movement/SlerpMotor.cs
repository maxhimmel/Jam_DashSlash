using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Movement
{
	using Player;
	using Xam.Utility.Extensions;

	public class SlerpMotor : MonoBehaviour,
		IInterpolationMovement,
		ITrajectoryControllerContainer
	{
		public Vector3 Position => m_body.position;
		public bool IsMoving => m_moveTween.IsActive() || m_body.velocity.sqrMagnitude > 0.01f;
		public Vector3 Velocity => (m_body.position - m_prevPosition) / Time.deltaTime;

		[SerializeField] private bool m_clearVelocityOnMove = true;

		[Space]
		[SerializeField] private Ease m_ease = Ease.OutQuad;
		[SerializeField] private float m_lerpDuration = 0.5f;

		[Space]
		[SerializeField] private AnimationCurve m_curveOffset;
		[SerializeField] private float m_maxCurveOffset = 30;
		[SerializeField] private float m_maxMidpointDistance = 7;

		private ITrajectoryController m_trajectoryController;
		private Rigidbody2D m_body;
		private float m_tweenTimer = 0;
		private Tweener m_moveTween;
		private Vector2 m_prevPosition;
		private Vector3 m_startPos;
		private Vector3 m_endPos;
		private Vector3 m_slerpOffset;
		private DragArgs m_currentDragArgs;
		private List<Vector3> m_dragPoints = new List<Vector3>();

		public void InstallTrajectory( ITrajectoryController controller )
		{
			if ( m_trajectoryController != null )
			{
				m_trajectoryController.DragStarted -= OnDragStarted;
				m_trajectoryController.DragUpdated -= OnDragUpdated;
			}

			if ( controller != null )
			{
				controller.DragStarted += OnDragStarted;
				controller.DragUpdated += OnDragUpdated;
			}

			m_trajectoryController = controller;
		}

		public void SetDuration( float duration )
		{
			m_lerpDuration = duration;
		}

		public void SetEase( Ease ease )
		{
			m_ease = ease;
		}

		public void ClearMovement()
		{
			m_moveTween.Kill();
			m_body.velocity = Vector2.zero;
		}

		public void SetDesiredVelocity( Vector3 direction )
		{
			if ( IsMoving )
			{
				m_moveTween.Kill();

				if ( m_clearVelocityOnMove )
				{
					m_body.velocity = Vector2.zero;
				}
			}

			m_tweenTimer = 0;
			m_startPos = Position;
			m_endPos = direction + Position;
			m_slerpOffset = GetOffsetPosition( direction );

			m_moveTween = DOTween.To( () => m_tweenTimer, tweenTimer => m_tweenTimer = tweenTimer, 1, m_lerpDuration )
				.SetUpdate( UpdateType.Manual )
				.SetEase( m_ease );
		}

		private Vector3 GetOffsetPosition( Vector3 direction )
		{
			var signedMidpointDistance = GetSlerpOffsetSignedDistance();
			var offsetSign = Mathf.Sign( signedMidpointDistance );
			var midpointDistance = signedMidpointDistance * offsetSign;

			var midpointLerp = Mathf.Clamp01( midpointDistance / m_maxMidpointDistance );
			var midpointCurve = m_curveOffset.Evaluate( midpointLerp );
			var offsetDistance = Mathf.LerpUnclamped( 0, m_maxCurveOffset, midpointCurve );

			var midpoint = (m_startPos + m_endPos) / 2f;
			var offsetDir = Quaternion.AngleAxis( 90, Vector3.forward ) * direction.normalized;
			var offsetPos = midpoint + offsetDir * (offsetDistance * offsetSign);

			return transform.InverseTransformPoint( offsetPos );
		}

		private float GetSlerpOffsetSignedDistance()
		{
			var nearestZero = Mathf.Infinity;
			var mostAlignedPoint = Vector3.zero;
			var midpoint = (m_currentDragArgs.Start + m_currentDragArgs.End) / 2f;
			var dragDirection = m_currentDragArgs.Vector;

			for ( int idx = 1; idx < m_dragPoints.Count - 1; ++idx )
			{
				var dragPos = m_dragPoints[idx];
				var midPointToDragPos = (dragPos - midpoint);
				float dot = Mathf.Abs( Vector3.Dot( midPointToDragPos, dragDirection ) );

				if ( dot < nearestZero )
				{
					nearestZero = dot;
					mostAlignedPoint = dragPos;
				}
			}

			var cross = Vector3.Cross( dragDirection, Vector3.forward );
			var signDir = Mathf.Sign( Vector3.Dot( cross, (mostAlignedPoint - midpoint) ) );
			float distFromMidpoint = (mostAlignedPoint - midpoint).magnitude;

			return m_dragPoints.Count <= 3
				? 0
				: distFromMidpoint * signDir;
		}

		private void FixedUpdate()
		{
			m_prevPosition = m_body.position;

			if ( m_moveTween.IsActive() )
			{
				m_moveTween.ManualUpdate( Time.deltaTime, Time.unscaledDeltaTime );
				m_body.MovePosition( GetNextPosition() );
			}
		}

		private Vector3 GetNextPosition()
		{
			var midpoint = (m_startPos + m_endPos) / 2f + m_slerpOffset;
			var relativeStart = m_startPos - midpoint;
			var relativeEnd = m_endPos - midpoint;

			return Vector3.SlerpUnclamped( relativeStart, relativeEnd, m_tweenTimer ) + midpoint;
		}

		private void OnDragStarted( object sender, DragArgs e )
		{
			m_currentDragArgs = e;

			m_dragPoints.Clear();
			m_dragPoints.Add( e.Start );
		}

		private void OnDragUpdated( object sender, DragArgs e )
		{
			m_currentDragArgs = e;

			var prevDragPoint = m_dragPoints[m_dragPoints.Count - 1];
			if ( (prevDragPoint - e.End).sqrMagnitude > 0.01f )
			{
				m_dragPoints.Add( e.End );
			}
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
			m_trajectoryController = GetComponentInChildren<ITrajectoryController>();

			m_currentDragArgs = new DragArgs( m_body.position, m_body.position );
		}

		private void OnDestroy()
		{
			m_trajectoryController.DragStarted -= OnDragStarted;
			m_trajectoryController.DragUpdated -= OnDragUpdated;

			if ( m_moveTween.IsActive() )
			{
				m_moveTween.Kill();
			}
		}

		//private void OnDrawGizmos()
		//{
		//	if ( m_slerpCenter != null )
		//	{
		//		Vector3 centerToSelf = m_startPos - m_slerpCenter.position;
		//		Gizmos.DrawWireSphere( m_slerpCenter.position, centerToSelf.magnitude );
		//	}
		//}
	}
}
