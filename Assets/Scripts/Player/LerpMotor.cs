using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DashSlash.Gameplay
{
    public class LerpMotor : MonoBehaviour
    {
		public Vector3 Position => m_body.position;
		public bool IsMoving => m_moveTween.IsActive() || m_body.velocity.sqrMagnitude > 0.01f;

		[SerializeField] private bool m_clearVelocityOnMove = true;

		[Space]
		[SerializeField] private Ease m_ease = Ease.OutQuad;
		[SerializeField] private float m_lerpDuration = 0.5f;

        private Rigidbody2D m_body;
		private Tweener m_moveTween;

		public void SetDuration( float duration )
		{
			m_lerpDuration = duration;
		}

		public void SetEase( Ease ease )
		{
			m_ease = ease;
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

			Vector3 endPos = direction + Position;
			m_moveTween = m_body.DOMove( endPos, m_lerpDuration )
				.SetUpdate( UpdateType.Fixed )
				.SetEase( m_ease );
		}

		public void ClearMovement()
		{
			m_moveTween.Kill();
			m_body.velocity = Vector2.zero;
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
