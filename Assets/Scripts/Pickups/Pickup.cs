using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay
{
    public class Pickup : MonoBehaviour
    {
		public bool IsCleaningUp { get; private set; }

		[SerializeField] private float m_lifetime = 3;
		[SerializeField] private float m_launchTorque = 10;

		[Space]
		[SerializeField] private float m_spawnInvincibilityDuration = 0.25f;

		private Rigidbody2D m_body;
		private Collider2D m_collider;

		public void Launch( Vector3 velocity )
		{
			m_body.AddForce( velocity, ForceMode2D.Impulse );

			int torqueDir = (Random.value >= 0.5f) ? 1 : -1;
			float torque = m_launchTorque * torqueDir;
			m_body.AddTorque( torque, ForceMode2D.Impulse );

			this.StartWaitingForSeconds( m_lifetime, Cleanup );
			this.StartWaitingForSeconds( m_spawnInvincibilityDuration, SetCollisionActive, true );
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( IsCleaningUp ) { return; }
			Cleanup();
		}

		private void Cleanup()
		{
			IsCleaningUp = true;
			Destroy( gameObject );
		}

		private void SetCollisionActive( bool isActive )
		{
			m_collider.enabled = isActive;
		}

		private void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
			m_collider = GetComponentInChildren<Collider2D>( true );
		}
	}
}
