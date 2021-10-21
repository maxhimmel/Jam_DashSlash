using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Weapons
{
	[RequireComponent( typeof( Collider2D ) )]
	public class HitBox : MonoBehaviour
    {
		[SerializeField] private DamageDatum m_damageData = new DamageDatum();

		private Collider2D m_collider;

		public void SetOwner( Transform owner )
		{
			m_damageData.Instigator = owner;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			Rigidbody2D otherBody = collision.attachedRigidbody;
			if ( otherBody == null ) { return; }

			if ( otherBody.TryGetDamageable( out var damageable ) )
			{
				damageable.TakeDamage( m_damageData );
			}
		}

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
