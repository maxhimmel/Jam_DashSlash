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
			if ( m_moveTween.IsActive() )
			{
				m_moveTween.Kill();
			}

			Vector3 endPos = direction + Position;
			m_moveTween = m_body.DOMove( endPos, m_lerpDuration )
				.SetEase( m_ease );
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
