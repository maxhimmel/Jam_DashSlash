using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay
{
	using Utility;

	public class LootSpawner : MonoBehaviour
    {
		public event System.EventHandler<Pickup> PickupSpawned;

        [SerializeField] private Pickup m_pickupPrefab = default;

        [Space]
        [SerializeField] private RandomFloatRange m_launchForceRange = new RandomFloatRange( 5, 10 );
        [SerializeField] private RandomIntRange m_spawnAmountRange = new RandomIntRange( 3, 8 );

		private IDirection m_launchRotation;

		public int Spawn( Vector3 position )
		{
			int spawnAmount = m_spawnAmountRange.Evaluate();
			for ( int idx = 0; idx < spawnAmount; ++idx )
			{
				Pickup newPickup = CreatePickup( position );
				LaunchPickup( newPickup, idx, spawnAmount );
			}

			return spawnAmount;
		}

		private Pickup CreatePickup( Vector3 position )
		{
			int randAngle = Random.Range( 0, 360 );
			Quaternion spawnRot = Quaternion.Euler( 0, 0, randAngle );

			var newPickup = Instantiate( m_pickupPrefab, position, spawnRot );

			PickupSpawned?.Invoke( this, newPickup );

			return newPickup;
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
