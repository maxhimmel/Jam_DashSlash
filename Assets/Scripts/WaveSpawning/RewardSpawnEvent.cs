namespace DashSlash.Gameplay.WaveSpawning
{
	public class RewardSpawnEvent : SpawnEvent<LootSpawner>
	{
		protected override void OnSpawned( LootSpawner spawnable )
		{
			base.OnSpawned( spawnable );

			spawnable.Spawn( spawnable.transform.position );

			Cleanup( spawnable );
		}

		private void Cleanup( LootSpawner spawner )
		{
			Destroy( spawner.gameObject );
		}
	}
}
