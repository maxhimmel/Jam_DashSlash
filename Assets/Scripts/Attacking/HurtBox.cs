using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay
{
	[RequireComponent( typeof( Collider2D ) )]
    public class HurtBox : MonoBehaviour
    {
        private Collider2D m_collider;

		private void OnEnable()
		{
			m_collider.enabled = true;
		}

		private void OnDisable()
		{
			m_collider.enabled = false;
		}

		private void Awake()
		{
			m_collider = GetComponent<Collider2D>();
		}
	}
}
