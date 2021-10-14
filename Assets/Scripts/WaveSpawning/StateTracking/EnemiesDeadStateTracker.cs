using DashSlash.Gameplay.Enemies;
using DashSlash.Gameplay.EventQueues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.WaveSpawning
{
    public class EnemiesDeadStateTracker : EnemySpawnStateTracker
    {
		public override PlayState State => IsAnyEnemyAlive() ? PlayState.Playing : PlayState.Done;

		private int m_deadEnemiesCount = 0;

		public override void PostPlay()
		{
			// Intentionally blank
				// ...
		}

		public override void PrePlay( int expectedSpawnCount )
		{
			if ( IsAnyEnemyAlive() )
			{
				Debug.LogWarning( $"Resetting enemy death count while tracked enemies live!", this );
			}

			base.PrePlay( expectedSpawnCount );

			m_deadEnemiesCount = 0;
		}

		public override void Spawned( Enemy enemy )
		{
			base.Spawned( enemy );

			enemy.Died += OnEnemyDied;
		}

		private void OnEnemyDied( object sender, System.EventArgs e )
		{
			Enemy deadEnemy = sender as Enemy;
			deadEnemy.Died -= OnEnemyDied;

			++m_deadEnemiesCount;
		}

		private bool IsAnyEnemyAlive()
		{
			return m_expectedSpawnCount > m_deadEnemiesCount;
		}
	}
}
