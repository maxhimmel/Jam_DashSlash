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

	public class EnemySpawnEvent : MonoBehaviour, IEvent
	{
		public virtual PlayState State => m_stateTracker.State;

		[SerializeField, Min( 1 )] private int m_spawnsPerDelay = 1;
		[SerializeField, Min( 0 )] private float m_nextSpawnDelay = 0;
		[SerializeField] protected RandomIntRange m_spawnRange = new RandomIntRange( 1, 4 );

		[Space]
		[SerializeField, Min( 0 )] private float m_spawnAnticDuration = 0.5f;
		[SerializeField] private GameObject m_spawnAnticPrefab = default;

		[Space]
		[SerializeField] protected InstancedPlacement m_placement = default;

		protected IFactory<Enemy> m_enemyFactory;
		protected EnemySpawnStateTracker m_stateTracker;
		private Coroutine m_spawnRoutine;

		public virtual void Play()
		{
			int numSpawns = m_spawnRange.Evaluate();

			this.TryStopCoroutine( ref m_spawnRoutine );
			m_spawnRoutine = StartCoroutine( UpdateSpawning( numSpawns ) );
		}

		protected virtual IEnumerator UpdateSpawning( int numSpawns )
		{
			this.Log( $"Spawning: {numSpawns}" );

			m_stateTracker.PrePlay( numSpawns );

			int spawnCounter = 0;
			while ( spawnCounter < numSpawns )
			{
				EnemySpawnData spawnData = new EnemySpawnData();

				for ( int idx = 0; idx < m_spawnsPerDelay; ++idx )
				{
					if ( spawnCounter >= numSpawns ) { break; }

					m_placement.GetNextOrientation( spawnCounter, numSpawns, out Vector3 spawnPos, out Quaternion spawnRot );

					GameObject antic = Instantiate( m_spawnAnticPrefab, spawnPos, spawnRot );
					spawnData.Antics.Add( antic );

					++spawnCounter;
				}

				StartCoroutine( UpdateSpawnAntic( spawnData ) );

				if ( m_nextSpawnDelay > 0 )
				{
					yield return new WaitForSeconds( m_nextSpawnDelay );
				}
			}

			m_spawnRoutine = null;

			m_stateTracker.PostPlay();
		}

		private IEnumerator UpdateSpawnAntic( EnemySpawnData spawnData )
		{
			if ( m_spawnAnticDuration > 0 )
			{
				yield return new WaitForSeconds( m_spawnAnticDuration );
			}

			foreach ( var antic in spawnData.Antics )
			{
				Enemy newEnemy = m_enemyFactory.Create( antic.transform.position, antic.transform.rotation );
				OnSpawned( newEnemy );

				Destroy( antic );
			}
		}

		protected virtual void OnSpawned( Enemy enemy )
		{
			m_stateTracker.Spawned( enemy );
		}

		protected virtual void Awake()
		{
			m_enemyFactory = GetComponentInChildren<IFactory<Enemy>>();
			m_stateTracker = GetComponentInChildren<EnemySpawnStateTracker>();

			if ( m_placement == null )
			{
				m_placement = GetComponentInChildren<InstancedPlacement>();
			}
		}
	}

	class EnemySpawnData
	{
		public List<GameObject> Antics = new List<GameObject>();
	}
}
