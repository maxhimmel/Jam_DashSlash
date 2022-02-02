using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Xam.Gameplay.Patterns;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay.Enemies.Factories
{
	public class WeightedEnemyFactory : MonoBehaviour, IFactory<Enemy>
	{
		public event EventHandler<Enemy> Created;

		[SerializeField] private WeightedEnemyList m_enemies = new WeightedEnemyList();

		public Enemy Create( Vector3 position = default, Quaternion rotation = default, Transform parent = null )
		{
			Enemy randomEnemy = m_enemies.GetRandomItem();
			if ( randomEnemy == null ) 
			{
				throw new NoNullAllowedException( nameof( randomEnemy ) );
			}

			Enemy newEnemy = Instantiate( randomEnemy, position, rotation, parent );

			Created?.Invoke( this, newEnemy );

			return newEnemy;
		}

		protected virtual void Awake()
		{
			m_enemies.Init();
		}
	}
}
