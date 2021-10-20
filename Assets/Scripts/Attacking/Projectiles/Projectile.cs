using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay.Vfx;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Weapons
{
    public class Projectile : MonoBehaviour
    {
		public bool IsCleaningUp { get; private set; }

		private Vector3 Direction => transform.up;

		[SerializeField] private float m_lifetime = 1;

		[Header( "Cleanup" )]
		[SerializeField] private float m_cleanupDuration = 0.5f;

		private Transform m_owner;
        private Rigidbody2D m_body;
		private DamageDatum m_damageData = new DamageDatum();

		public void SetOwner( Transform owner )
		{
			m_owner = owner;

			m_damageData.Instigator = owner;
			m_damageData.DamageCauser = this.transform;
		}

		public void Fire( float force )
		{
			Vector3 velocity = Direction * force;
			m_body.AddForce( velocity, ForceMode2D.Impulse );

			this.StartWaitingForSeconds( m_lifetime, Cleanup );
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( IsCleaningUp ) { return; }
			Cleanup();

			Rigidbody2D otherBody = collision.attachedRigidbody;
			if ( otherBody.TryGetDamageable( out IDamageable damageable ) )
			{
				damageable.TakeDamage( m_damageData );
			}
		}

		private void Cleanup()
		{
			IsCleaningUp = true;
			m_body.simulated = false;

			MeshFadeEmancipation meshFader = gameObject.AddComponent<MeshFadeEmancipation>();
			meshFader.SetDuration( m_cleanupDuration );
			meshFader.Emancipate();

			Destroy( gameObject, m_cleanupDuration );
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
