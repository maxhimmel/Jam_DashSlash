using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Vfx.Googly
{
    public class GoogleEye : MonoBehaviour
    {
        [Tooltip( "This is the white part of an eyeball. (Who knew?)" )]
        [SerializeField] private Transform m_sclera = default;
        [SerializeField] private Transform m_pupil = default;

        public void SetDesiredLookDirection( Vector3 direction )
		{
            direction = m_pupil.InverseTransformDirection( direction );

            float maxRadius = m_sclera.lossyScale.x / 2f - m_pupil.lossyScale.x / 2f;
            Vector3 pupilPos = Vector3.ClampMagnitude( direction, maxRadius );

            m_pupil.localPosition = pupilPos;
		}
    }
}
