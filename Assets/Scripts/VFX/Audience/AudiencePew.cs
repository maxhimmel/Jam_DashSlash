using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;
using Xam.Gameplay;
using Xam.Gameplay.Patterns;
using Cinemachine;

namespace DashSlash.Vfx.Audiences
{
    using Gameplay;
    using Gameplay.Player;
    using Gameplay.Enemies;

    public class AudiencePew : MonoBehaviour
    {
		[Header( "Spawning" )]
        [SerializeField] private int m_count = 10;

		private IPlacement m_placement;
        private IFactory<GameObject> m_spectatorFactory;

		private void Start()
		{
			for ( int idx = 0; idx < m_count; ++idx )
			{
				m_placement.GetNextOrientation( idx, m_count, out Vector3 spawnPos, out Quaternion spawnRot );
				
				GameObject spectator = m_spectatorFactory.Create( spawnPos, spawnRot, transform );
				spectator.SetActive( true );
			}
		}

		private void Awake()
		{
            m_placement = GetComponentInChildren<IPlacement>();
            m_spectatorFactory = GetComponentInChildren<IFactory<GameObject>>();
		}
	}
}
