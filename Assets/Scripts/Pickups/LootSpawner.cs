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
			Vector3 spawnPos = transform.position;

			int spawnAmount = m_spawnAmountRange.Evaluate();
			for ( int idx = 0; idx < spawnAmount; ++idx )
			{
				int randAngle = Random.Range( 0, 360 );
				Quaternion spawnRot = Quaternion.Euler( 0, 0, randAngle );
				Pickup newPickup = Instantiate( m_pickupPrefab, spawnPos, spawnRot );

				Vector3 launchDir = Random.insideUnitCircle.normalized;
				float launchForce = m_launchForceRange.Evaluate();
				newPickup.Launch( launchDir * launchForce );
			}
		}
	}
}
