using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Player
{
	using Movement;

	public class DamageHandler : MonoBehaviour, IDamageable
	{
		private LerpMotor m_motor;
		private Rigidbody2D m_body;

		public void TakeDamage( DamageDatum dmgData )
		{
			m_motor.ClearMovement();
		}

		private void Awake()
		{
			m_motor = GetComponent<LerpMotor>();
			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
