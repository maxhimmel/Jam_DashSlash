using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay
{
    public class LifetimeDestroyer : MonoBehaviour
    {
		[SerializeField, Min( 0 )] private float m_lifetimeDuration = 8;
		[SerializeField] private GameObject m_gameObjectOverride = default;

		private void Start()
		{
			GameObject destroyMe = GetGameObjectToDestroy();
			ApplyDestroy( destroyMe );
		}

		private GameObject GetGameObjectToDestroy()
		{
			return m_gameObjectOverride != null
				? m_gameObjectOverride
				: gameObject;
		}

		private void ApplyDestroy( GameObject destroyMe )
		{
			if ( m_lifetimeDuration > 0 )
			{
				Destroy( destroyMe, m_lifetimeDuration );
			}
			else
			{
				Destroy( destroyMe );
			}
		}
	}
}
