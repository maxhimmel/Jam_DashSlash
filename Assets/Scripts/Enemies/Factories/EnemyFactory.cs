using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay.Patterns;

namespace DashSlash.Gameplay.Enemies.Factories
{
    public class EnemyFactory : Factory<Enemy>
    {
        public void SetEnemyPrefab( Enemy prefab )
		{
            m_prefab = prefab;
		}
    }
}
