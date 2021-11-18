using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Vfx.Googly
{
    public class GooglyEyesController : MonoBehaviour
    {
		[SerializeField] private float m_dazedSpinSpeed = 720;

        private GooglyEye[] m_eyes = default;

		public void SetDesiredLookDirection( Vector3 direction )
		{
			foreach ( var eye in m_eyes )
			{
				eye.SetDesiredLookDirection( direction );
			}
		}

		public void PlayDazedAnim( float duration )
		{
			Vector3 center = transform.position;
			Vector3 right = transform.right;

			foreach ( var eye in m_eyes )
			{
				Vector3 eyeToCenter = center - eye.transform.position;
				float dir = -1 * Mathf.Sign( Vector3.Dot( eyeToCenter, right ) );

				eye.PlayDazedAnim( duration, m_dazedSpinSpeed * dir );
			}
		}

		private void Awake()
		{
			m_eyes = GetComponentsInChildren<GooglyEye>();
		}
	}
}
