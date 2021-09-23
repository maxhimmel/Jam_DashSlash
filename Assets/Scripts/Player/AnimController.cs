using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DashSlash.Gameplay.Player.Animation
{
    public class AnimController : MonoBehaviour
    {
		private Transform Model => transform;

		[Header( "Prepare Dash" )]
		[SerializeField] private float m_punchStrength = 25;
		[SerializeField] private int m_punchVibrato = 5;
		[SerializeField] private float m_punchElasticity = 1;

		[Header( "VFX" )]
		[SerializeField] private ParticleSystem m_dashVfx = default;

		private Tweener m_rotationAnim;

		public void PlayPrepareDashAnim( float duration )
		{
			if ( m_rotationAnim.IsActive() )
			{
				m_rotationAnim.Kill( true );
			}
			m_rotationAnim = Model.DOPunchRotation( Vector3.forward * m_punchStrength, duration, m_punchVibrato, m_punchElasticity );
		}

		public void PlayDashVfx( float duration, float radius )
		{
			ParticleSystem.MainModule dashModule = m_dashVfx.main;
			dashModule.startSize = radius * 2f;
			dashModule.startLifetime = duration;

			m_dashVfx.Play( true );
		}
	}
}
