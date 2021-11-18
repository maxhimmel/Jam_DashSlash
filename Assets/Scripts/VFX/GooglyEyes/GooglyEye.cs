using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

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
        private Coroutine m_dazedRoutine;

        public void SetDesiredLookDirection( Vector3 direction )
		{
            m_desiredLookDirection = m_pupil.InverseTransformDirection( direction );
		}

		private void LateUpdate()
        {
            if ( CanApplyLookDirection() )
            {
                ApplyLookDirection();
            }
        }

        private bool CanApplyLookDirection()
        {
            return m_dazedRoutine == null;
        }

        private void ApplyLookDirection()
        {
            m_pupil.localPosition = Vector3.SmoothDamp( 
                m_pupil.localPosition, LookDirToPupilPos( m_desiredLookDirection ), ref m_lookVelocity, m_lookDamping 
            );
        }

        public void PlayDazedAnim( float duration, float spinSpeed )
		{
            this.TryStopCoroutine( ref m_dazedRoutine );
            m_dazedRoutine = StartCoroutine( UpdateDazedAnim( duration, spinSpeed ) );
		}

        private IEnumerator UpdateDazedAnim( float duration, float spinSpeed )
		{
            float angle = 0;
            float timer = 0;
            while ( timer < 1 )
			{
                timer += Time.deltaTime / duration;
                angle += spinSpeed * Time.deltaTime;

                Vector3 dir = Quaternion.AngleAxis( angle, Vector3.forward ) * Vector3.up;
                m_pupil.localPosition = LookDirToPupilPos( dir );

                yield return null;
			}

            SetDesiredLookDirection( Vector3.zero );

            m_dazedRoutine = null;
		}

        private Vector3 LookDirToPupilPos( Vector3 direction )
        {
            float maxRadius = m_sclera.lossyScale.x / 2f - m_pupil.lossyScale.x / 2f;
            return Vector3.ClampMagnitude( direction, maxRadius );
        }
	}
}
