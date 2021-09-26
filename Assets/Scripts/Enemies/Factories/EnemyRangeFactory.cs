using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay.Enemies.Factories
{
    [RequireComponent( typeof( EnemyFactory ), typeof( InstancedPlacement ) )]
    public class EnemyRangeFactory : MonoBehaviour
    {
        [SerializeField] private RandomIntRange m_spawnRange = new RandomIntRange( 1, 1 );

        private EnemyFactory m_factory;
		private IPlacement m_placement;

        public Enemy[] CreateRange()
		{
			int spawnCount = m_spawnRange.Evaluate();
			Enemy[] results = new Enemy[spawnCount];

			for ( int idx = 0; idx < spawnCount; ++idx )
			{
				m_placement.GetNextOrientation( idx, spawnCount, out Vector3 spawnPos, out Quaternion spawnRot );

				results[idx] = m_factory.Create( spawnPos, spawnRot );
			}

			return results;
		}

		private void Awake()
		{
			m_factory = GetComponent<EnemyFactory>();
			m_placement = GetComponent<IPlacement>();
		}
	}
}
