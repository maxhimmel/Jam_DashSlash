using DashSlash.Gameplay.Enemies.Factories;
using System;
using UnityEngine;
using Xam.Utility.Extensions;
using Xam.Utility.Juicy;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay.Enemies
{
    public class Hive : DirectionMover
    {
		private bool IsExpired => m_lifetimeExpiration > 0 && m_lifetimeExpiration <= Time.timeSinceLevelLoad;

		[Header( "Hive" )]
		[SerializeField] private float m_lifetime = 5;
		[SerializeField] private float m_expirationAnticDuration = 0.5f;

		[Header( "Child Enemies" )]
		[SerializeField] private RandomFloatRange m_spawnForceRange = new RandomFloatRange( 4, 8 );
		[SerializeField] private RandomFloatRange m_spawnTorqueRange = new RandomFloatRange( 90, 180 );

		[Space]
		[SerializeField] private RandomFloatRange m_angrySpawnForceRange = new RandomFloatRange( 10, 14 );
		[SerializeField] private RandomFloatRange m_angrySpawnTorqueRange = new RandomFloatRange( 90, 180 );

		private float m_lifetimeExpiration = -1;
		private EnemyRangeFactory m_rangeFactory;
		private ColorBlinker m_expirationAntic;
		private Coroutine m_anticRoutine;

		protected override void UpdateState()
		{
			base.UpdateState();

			if ( IsExpired )
			{
				Destroy( gameObject );
			}
		}

		protected override void OnAwokenFromSpawn()
		{
			base.OnAwokenFromSpawn();

			m_lifetimeExpiration = m_lifetime + Time.timeSinceLevelLoad;

			float anticDelay = m_lifetime - m_expirationAnticDuration;
			m_anticRoutine = this.StartWaitingForSeconds( anticDelay, m_expirationAntic.Play );
		}

		protected override void OnSliced( object sender, EventArgs e )
		{
			base.OnSliced( sender, e );

			m_lifetimeExpiration = -1;

			this.TryStopCoroutine( ref m_anticRoutine );
			m_expirationAntic.Stop();
		}

		protected override void OnDied()
		{
			base.OnDied();

			Enemy[] enemies = m_rangeFactory.CreateRange();
			foreach ( Enemy enemy in enemies )
			{
				Rigidbody2D body = enemy.GetComponent<Rigidbody2D>();
				if ( body != null )
				{
					var forceRange = IsExpired ? m_angrySpawnForceRange : m_spawnForceRange;
					var torqueRange = IsExpired ? m_angrySpawnTorqueRange : m_spawnTorqueRange;
					LaunchEnemy( body, forceRange, torqueRange );
				}

				if ( enemy is Swarmer swarmer )
				{
					if ( IsExpired )
					{
						swarmer.SetAngry();
					}
				}
			}
		}

		private void LaunchEnemy( Rigidbody2D enemyBody, RandomFloatRange forceRange, RandomFloatRange torqueRange )
		{
			Vector3 forceDir = enemyBody.transform.up;
			Vector3 spawnVelocity = forceDir * forceRange.Evaluate();
			enemyBody.AddForce( spawnVelocity, ForceMode2D.Impulse );

			float torque = RandomUtility.Sign() * torqueRange.Evaluate();
			enemyBody.AddTorque( torque, ForceMode2D.Impulse );
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_rangeFactory = GetComponentInChildren<EnemyRangeFactory>();
			m_expirationAntic = GetComponentInChildren<ColorBlinker>();
		}
	}
}
