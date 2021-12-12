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

	public class SpawnEvent<TSpawnable> : MonoBehaviour, IEvent
		where TSpawnable : Component
	{
		public virtual PlayState State => m_stateTracker.State;

		protected float SpawnAnticDuration => m_spawnAnticPrefab != null ? m_spawnAnticPrefab.Duration : 0;

		[SerializeField, Min( 1 )] private int m_spawnsPerDelay = 1;
		[SerializeField, Min( 0 )] private float m_nextSpawnDelay = 0;
		[SerializeField] protected RandomIntRange m_spawnRange = new RandomIntRange( 1, 4 );

		[Space]
		[SerializeField] private SpawnAntic m_spawnAnticPrefab = default;

		[Space]
		[SerializeField] protected InstancedPlacement m_placement = default;

		protected IFactory<TSpawnable> m_factory;
		protected SpawnStateTracker<TSpawnable> m_stateTracker;
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
				SpawnData spawnData = new SpawnData();

				for ( int idx = 0; idx < m_spawnsPerDelay; ++idx )
				{
					if ( spawnCounter >= numSpawns ) { break; }

					m_placement.GetNextOrientation( spawnCounter, numSpawns, out Vector3 spawnPos, out Quaternion spawnRot );

					spawnData.Positions.Add( spawnPos );
					spawnData.Rotations.Add( spawnRot );
					if ( m_spawnAnticPrefab != null )
					{
						SpawnAntic antic = Instantiate( m_spawnAnticPrefab, spawnPos, spawnRot );
						spawnData.Antics.Add( antic );
					}


					++spawnCounter;
				}

				StartCoroutine( UpdateSpawnAntic( spawnData ) );

				if ( spawnCounter < numSpawns && m_nextSpawnDelay > 0 )
				{
					yield return new WaitForSeconds( m_nextSpawnDelay );
				}
			}

			m_spawnRoutine = null;

			m_stateTracker.PostPlay();
		}

		private IEnumerator UpdateSpawnAntic( SpawnData spawnData )
		{
			if ( SpawnAnticDuration > 0 )
			{
				yield return new WaitForSeconds( SpawnAnticDuration );
			}

			for ( int idx = 0; idx < spawnData.Positions.Count; ++idx )
			{
				Vector3 spawnPos = spawnData.Positions[idx];
				Quaternion spawnRot = spawnData.Rotations[idx];

				TSpawnable newSpawnable = m_factory.Create( spawnPos, spawnRot );
				OnSpawned( newSpawnable );

				if ( idx < spawnData.Antics.Count )
				{
					SpawnAntic antic = spawnData.Antics[idx];
					Destroy( antic.gameObject );
				}
			}
		}

		protected virtual void OnSpawned( TSpawnable spawnable )
		{
			m_stateTracker.Spawned( spawnable );
		}

		protected virtual void Awake()
		{
			m_factory = GetComponentInChildren<IFactory<TSpawnable>>();
			m_stateTracker = GetComponentInChildren<SpawnStateTracker<TSpawnable>>();

			if ( m_placement == null )
			{
				m_placement = GetComponentInChildren<InstancedPlacement>();
			}
		}

		class SpawnData
		{
			public List<SpawnAntic> Antics = new List<SpawnAntic>();
			public List<Vector3> Positions = new List<Vector3>();
			public List<Quaternion> Rotations = new List<Quaternion>();
		}
	}
}
