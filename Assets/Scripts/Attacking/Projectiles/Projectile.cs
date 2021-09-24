using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Weapons
{
    public class Projectile : MonoBehaviour
    {
		private Vector3 Direction => transform.up;

        private Rigidbody2D m_body;

		public void Fire( float force )
		{
			Vector3 velocity = Direction * force;
			m_body.AddForce( velocity, ForceMode2D.Impulse );
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
