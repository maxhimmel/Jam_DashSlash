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

			HandleMatryoshkaEnemy( deadEnemy );
		}

		private void HandleMatryoshkaEnemy( Enemy deadEnemy )
		{
			var matryoshka = deadEnemy.GetComponent<IMatryoshkaEnemy>();
			if ( matryoshka == null ) { return; }

			var children = matryoshka.GetChildren();
			if ( children == null ) 
			{
				Debug.LogWarning(
					$"IMatryoshkaEnemy ({deadEnemy.name}) had no children! " +
					$"Was this expected? " +
					$"Or was GetChildren() called out of order?", this 
				);

				return; 
			}

			m_expectedSpawnCount += children.Length;
			foreach ( var child in children )
			{
				child.Died += OnEnemyDied;
			}
		}

		private bool IsAnyEnemyAlive()
		{
			return m_expectedSpawnCount > m_deadEnemiesCount;
		}
	}
}
