using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;
using Xam.Gameplay.Patterns;
using Xam.Utility.Randomization;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.WaveSpawning
{
    using EventQueues;
	using Enemies;
	using Enemies.Factories;

	public class EnemySpawnEvent : MonoBehaviour, IEvent
	{
		public virtual PlayState State => m_stateTracker.State;

		[SerializeField, Min( 0 )] private float m_nextSpawnDelay = 0;
		[SerializeField] protected RandomIntRange m_spawnRange = new RandomIntRange( 1, 4 );

		protected IFactory<Enemy> m_enemyFactory;
		protected IPlacement m_placement;
		protected EnemySpawnStateTracker m_stateTracker;
		protected Coroutine m_spawnRoutine;

		public virtual void Play()
		{
			int numSpawns = m_spawnRange.Evaluate();

			this.TryStopCoroutine( ref m_spawnRoutine );
			m_spawnRoutine = StartCoroutine( UpdateSpawning( numSpawns ) );
		}

		protected virtual IEnumerator UpdateSpawning( int numSpawns )
		{
			m_stateTracker.PrePlay( numSpawns );

			for ( int idx = 0; idx < numSpawns; ++idx )
			{
				m_placement.GetNextOrientation( idx, numSpawns, out Vector3 spawnPos, out Quaternion spawnRot );
				Enemy newEnemy = m_enemyFactory.Create( spawnPos, spawnRot );

				OnSpawned( newEnemy );

				if ( m_nextSpawnDelay > 0 )
				{
					yield return new WaitForSeconds( m_nextSpawnDelay );
				}
			}

			m_spawnRoutine = null;

			m_stateTracker.PostPlay();
		}

		protected virtual void OnSpawned( Enemy enemy )
		{
			m_stateTracker.Spawned( enemy );
		}

		protected virtual void Awake()
		{
			m_enemyFactory = GetComponentInChildren<IFactory<Enemy>>();
			m_placement = GetComponentInChildren<IPlacement>();
			m_stateTracker = GetComponentInChildren<EnemySpawnStateTracker>();
		}
	}
}
