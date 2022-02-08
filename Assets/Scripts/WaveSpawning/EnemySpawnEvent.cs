namespace DashSlash.Gameplay.WaveSpawning
{
	using Enemies;

	public class EnemySpawnEvent : SpawnEvent<Enemy>
	{
		protected override void OnSpawned( Enemy spawnable )
		{
			base.OnSpawned( spawnable );
		}
	}
}
