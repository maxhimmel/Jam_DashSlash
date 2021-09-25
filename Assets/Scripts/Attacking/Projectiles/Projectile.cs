using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Weapons
{
    public class Projectile : MonoBehaviour
    {
		private Vector3 Direction => transform.up;

		[SerializeField] private float m_lifetime = 1;

        private Rigidbody2D m_body;

		public void Fire( float force )
		{
			Vector3 velocity = Direction * force;
			m_body.AddForce( velocity, ForceMode2D.Impulse );

			this.StartWaitingForSeconds( m_lifetime, Cleanup );
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			Cleanup();
		}

		private void Cleanup()
		{
			Destroy( gameObject );
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
