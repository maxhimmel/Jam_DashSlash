using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Vfx.Googly
{
    public class GooglyEye : MonoBehaviour
    {
        [SerializeField, Range( 0, 2 )] private float m_lookDamping = 0.15f;

        [Space]
        [Tooltip( "This is the white part of an eyeball. (Who knew?)" )]
        [SerializeField] private Transform m_sclera = default;
        [SerializeField] private Transform m_pupil = default;

        private Vector3 m_lookVelocity;
        private Vector3 m_desiredLookDirection;

        public void SetDesiredLookDirection( Vector3 direction )
		{
            m_desiredLookDirection = m_pupil.InverseTransformDirection( direction );
		}

		private void LateUpdate()
        {
            float maxRadius = m_sclera.lossyScale.x / 2f - m_pupil.lossyScale.x / 2f;
            Vector3 pupilPos = Vector3.ClampMagnitude( m_desiredLookDirection, maxRadius );

            m_pupil.localPosition = Vector3.SmoothDamp( m_pupil.localPosition, pupilPos, ref m_lookVelocity, m_lookDamping );
        }
	}
}
