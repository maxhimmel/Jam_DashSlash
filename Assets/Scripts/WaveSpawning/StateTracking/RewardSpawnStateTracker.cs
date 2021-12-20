using DashSlash.Gameplay.EventQueues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.WaveSpawning
{
    public class RewardSpawnStateTracker : SpawnStateTracker<LootSpawner>
    {
		public override PlayState State => IsAnyPickupAlive() ? PlayState.Playing : PlayState.Done;

		private LootSpawner m_lootSpawner;
		private int m_deadPickupsCount = 0;

		public void AddExpectedSpawns( int count )
		{
			m_expectedSpawnCount += count;
		}

		public override void Spawned( LootSpawner spawnable )
		{
			base.Spawned( spawnable );

			m_lootSpawner = spawnable;
			spawnable.PickupSpawned += OnPickupSpawned;
		}

		public override void PostPlay()
		{
			if ( m_lootSpawner != null )
			{
				m_lootSpawner.PickupSpawned -= OnPickupSpawned;
			}
		}

		private void OnPickupSpawned( object sender, Pickup pickup )
		{
			pickup.Died += OnPickupDied;
		}

		private void OnPickupDied( object sender, System.EventArgs args )
		{
			Pickup pickupSender = sender as Pickup;
			pickupSender.Died -= OnPickupDied;

			++m_deadPickupsCount;
		}

		private bool IsAnyPickupAlive()
		{
			return m_expectedSpawnCount > m_deadPickupsCount;
		}
	}
}
