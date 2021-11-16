using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay
{
	using Utility;
	using Enemies;

	public class LootSpawner : MonoBehaviour
    {
        [SerializeField] private Pickup m_pickupPrefab = default;

        [Space]
        [SerializeField] private RandomFloatRange m_launchForceRange = new RandomFloatRange( 5, 10 );
        [SerializeField] private RandomIntRange m_spawnAmountRange = new RandomIntRange( 3, 8 );

		private IDirection m_launchRotation;

		private void Start()
		{
            Enemy enemy = GetComponentInParent<Enemy>();
			if ( enemy != null )
			{
				enemy.Died += OnEnemyDied;
			}
		}

		private void OnEnemyDied( object sender, System.EventArgs e )
		{
			int spawnAmount = m_spawnAmountRange.Evaluate();
			for ( int idx = 0; idx < spawnAmount; ++idx )
			{
				Pickup newPickup = CreatePickup();
				LaunchPickup( newPickup, idx, spawnAmount );
			}
		}

		private Pickup CreatePickup()
		{
			int randAngle = Random.Range( 0, 360 );
			Quaternion spawnRot = Quaternion.Euler( 0, 0, randAngle );

			return Instantiate( m_pickupPrefab, transform.position, spawnRot );
		}

		private void LaunchPickup( Pickup pickup, int index, int spawnCount )
		{
			Vector3 launchDir = m_launchRotation.GetDirection( index, spawnCount );
			float launchForce = m_launchForceRange.Evaluate();

			pickup.Launch( launchDir * launchForce );
		}

		private void Awake()
		{
			m_launchRotation = GetComponent<IDirection>();
		}
	}
}
