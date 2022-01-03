using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Movement
{
	using Player;

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
		[SerializeField] private float m_minCurveOffset = 0;
		[SerializeField] private float m_maxCurveOffset = 1;

		[Header( "TESTING" )]
		[SerializeField] private Transform m_slerpCenter = default;

		private ITrajectoryController m_trajectoryController;
		private Rigidbody2D m_body;
		private Tweener m_moveTween;
		private Vector2 m_prevPosition;
		private Vector3 m_startPos;
		private Vector3 m_endPos;
		private Vector3 m_slerpOffset;
		private float m_lerpTimer = 0;

		public void InstallTrajectory( ITrajectoryController controller )
		{
			if ( m_trajectoryController != null )
			{

			}

			if ( controller != null )
			{

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
			SetDesiredVelocity( direction, 0 );
		}

		public void SetDesiredVelocity( Vector3 direction, float curveStrength )
		{
			if ( IsMoving )
			{
				m_moveTween.Kill();

				if ( m_clearVelocityOnMove )
				{
					m_body.velocity = Vector2.zero;
				}
			}

			m_lerpTimer = 0;
			m_startPos = Position;
			m_endPos = direction + Position;


			var midPoint = (m_startPos + m_endPos) / 2f;
			var offsetDir = Quaternion.AngleAxis( 90, Vector3.forward ) * direction;
			var offsetPos = midPoint + offsetDir * curveStrength;

			Debug.DrawRay( m_startPos, direction, Color.yellow, 5 );
			Debug.DrawRay( midPoint, offsetDir, Color.white, 5 );
			Debug.DrawRay( offsetPos, Vector3.up, Color.yellow, 5 );

			m_slerpOffset = transform.InverseTransformPoint( offsetPos );
			//m_slerpOffset = offsetDir * curveStrength;

			m_moveTween = DOTween.To( () => m_lerpTimer, tweenTimer => m_lerpTimer = tweenTimer, 1, m_lerpDuration )
				.SetUpdate( UpdateType.Manual )
				.SetEase( m_ease );
		}

		private void FixedUpdate()
		{
			m_prevPosition = m_body.position;

			if ( m_moveTween.IsActive() )
			{
				var midPoint = (m_startPos + m_endPos) / 2f + m_slerpOffset;
				var relativeStart = m_startPos - midPoint;
				var relativeEnd = m_endPos - midPoint;

				m_moveTween.ManualUpdate( Time.deltaTime, Time.unscaledDeltaTime );

				var nextPos = Vector3.SlerpUnclamped( relativeStart, relativeEnd, m_lerpTimer ) + midPoint;
				m_body.MovePosition( nextPos );
			}
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
			m_trajectoryController = GetComponentInChildren<ITrajectoryController>();
		}

		private void OnDestroy()
		{
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
