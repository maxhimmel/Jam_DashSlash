using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash
{
    public class CursorStateToggle : MonoBehaviour
    {
		[SerializeField] private CursorLockMode m_enabledState = CursorLockMode.Confined;
		[SerializeField] private CursorLockMode m_disabledState = CursorLockMode.None;

		private void OnEnable()
		{
			SetState( m_enabledState );
		}

		private void OnDisable()
		{
			SetState( m_disabledState );
		}

		public void SetState( CursorLockMode state )
		{
			Cursor.lockState = state;
		}
	}
}
