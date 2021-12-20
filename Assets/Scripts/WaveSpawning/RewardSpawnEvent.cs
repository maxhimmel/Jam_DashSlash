namespace DashSlash.Gameplay.WaveSpawning
{
	public class RewardSpawnEvent : SpawnEvent<LootSpawner>
	{
		protected override void OnSpawned( LootSpawner spawnable )
		{
			base.OnSpawned( spawnable );

			var spawnCount = spawnable.Spawn( spawnable.transform.position );
			NotifyStateTracker( spawnCount );

			Cleanup( spawnable );
		}

		private void NotifyStateTracker( int lootSpawnCount )
		{
			// NOTE: 
				// We're minusing 1 from the loot spawn count due to the loot spawner being included in the count.

			RewardSpawnStateTracker rewardTracker = m_stateTracker as RewardSpawnStateTracker;
			rewardTracker.AddExpectedSpawns( lootSpawnCount - 1 );
		}

		private void Cleanup( LootSpawner spawner )
		{
			Destroy( spawner.gameObject );
		}
	}
}
