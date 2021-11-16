using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Vfx.Googly
{
    public class GooglyEyesController : MonoBehaviour
    {
        private GoogleEye[] m_eyes = default;

		public void SetDesiredLookDirection( Vector3 direction )
		{
			foreach ( var eye in m_eyes )
			{
				eye.SetDesiredLookDirection( direction );
			}
		}

		private void Awake()
		{
			m_eyes = GetComponentsInChildren<GoogleEye>();
		}
	}
}
