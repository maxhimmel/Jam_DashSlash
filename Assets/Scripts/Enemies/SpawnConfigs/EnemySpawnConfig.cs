using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay.Patterns;

namespace DashSlash.Gameplay.Enemies.Utility
{
    public abstract class EnemySpawnConfig : MonoBehaviour
    {
        private IFactory<Enemy> m_factory;

		private void Awake()
		{
			m_factory = GetComponent<IFactory<Enemy>>();
			m_factory.Created += OnEnemyCreated;
		}

		protected abstract void OnEnemyCreated( object sender, Enemy e );

		protected virtual void OnDestroy()
		{
			if ( m_factory != null )
			{
				m_factory.Created -= OnEnemyCreated;
			}
		}
	}
}
