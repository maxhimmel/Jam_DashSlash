using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay.Enemies
{
    using Factories;

    public class Matryoshka : MonoBehaviour
    {
		[SerializeField] private RandomFloatRange m_spawnForceRange = new RandomFloatRange( 3, 6 );

		private Enemy m_enemy;
        private EnemyRangeFactory m_rangeFactory;

		private void OnDied( object sender, System.EventArgs e )
		{
			Enemy[] enemies = m_rangeFactory.CreateRange();
			foreach ( Enemy enemy in enemies )
			{
				Rigidbody2D body = enemy.GetComponent<Rigidbody2D>();
				if ( body == null ) { continue; }

				Vector3 forceDir = enemy.transform.up;
				Vector3 spawnVelocity = forceDir * m_spawnForceRange.Evaluate();
				body.AddForce( spawnVelocity, ForceMode2D.Impulse );
			}
		}

		private void Start()
		{
			m_enemy.Died += OnDied;
		}

		private void Awake()
		{
			m_enemy = GetComponentInParent<Enemy>();
			m_rangeFactory = GetComponent<EnemyRangeFactory>();
		}
	}
}
