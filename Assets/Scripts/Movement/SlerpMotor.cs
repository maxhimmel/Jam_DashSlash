using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Movement
{
    public class SlerpMotor : MonoBehaviour,
		IInterpolationMovement
	{
		public Vector3 Position => m_body.position;
		public bool IsMoving => m_moveTween.IsActive() || m_body.velocity.sqrMagnitude > 0.01f;
		public Vector3 Velocity => (m_body.position - m_prevPosition) / Time.deltaTime;

		[SerializeField] private bool m_clearVelocityOnMove = true;

		[Space]
		[SerializeField] private Ease m_ease = Ease.OutQuad;
		[SerializeField] private float m_lerpDuration = 0.5f;

		private Rigidbody2D m_body;
		private Tweener m_moveTween;
		private Vector2 m_prevPosition;

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

			m_startPos = m_body.position;
			m_endPos = direction + Position;

			if ( m_slerpCenter != null )
			{
				m_slerpCenterOffset = transform.InverseTransformPoint( m_slerpCenter.position );
			}

			m_lerpTimer = 0;
			m_moveTween = DOTween.To( () => m_lerpTimer, tweenTimer => m_lerpTimer = tweenTimer, 1, m_lerpDuration )
				.SetUpdate( UpdateType.Manual )
				.SetEase( m_ease );
		}

		[SerializeField] private Transform m_slerpCenter = default;
		[SerializeField] private Vector3 m_slerpOffset = Vector3.right;
		private float m_lerpTimer = 0;
		private Vector3 m_startPos, m_endPos, m_slerpCenterOffset;

		private void FixedUpdate()
		{
			m_prevPosition = m_body.position;

			if ( m_moveTween.IsActive() )
			{
				var offset = m_slerpCenter != null ?
					m_slerpCenterOffset : m_slerpOffset;

				var midPoint = (m_startPos + m_endPos) / 2f + offset;
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
