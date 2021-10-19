using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay
{
	using Enemies;

    public class LootSpawner : MonoBehaviour
    {
        [SerializeField] private Pickup m_pickupPrefab = default;

        [Space]
        [SerializeField] private RandomFloatRange m_launchForceRange = new RandomFloatRange( 5, 10 );
        [SerializeField] private RandomIntRange m_spawnAmountRange = new RandomIntRange( 3, 8 );

		private void Start()
		{
            Enemy enemy = GetComponentInParent<Enemy>();
			enemy.Died += OnEnemyDead;
		}

		private void OnEnemyDead( object sender, System.EventArgs e )
		{
			int spawnAmount = m_spawnAmountRange.Evaluate();
			for ( int idx = 0; idx < spawnAmount; ++idx )
			{
				Pickup newPickup = CreatePickup();
				LaunchPickup( newPickup );
			}
		}

		private Pickup CreatePickup()
		{
			int randAngle = Random.Range( 0, 360 );
			Quaternion spawnRot = Quaternion.Euler( 0, 0, randAngle );

			return Instantiate( m_pickupPrefab, transform.position, spawnRot );
		}

		private void LaunchPickup( Pickup pickup )
		{
			Vector3 launchDir = Random.insideUnitCircle.normalized;
			float launchForce = m_launchForceRange.Evaluate();

			pickup.Launch( launchDir * launchForce );
		}
	}
}
