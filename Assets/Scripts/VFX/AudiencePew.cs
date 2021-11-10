using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;
using Xam.Gameplay;
using Xam.Gameplay.Patterns;
using Cinemachine;

namespace DashSlash.Vfx
{
    using Gameplay;
    using Gameplay.Player;
    using Gameplay.Enemies;

    public class AudiencePew : MonoBehaviour
    {
		[Header( "Spawning" )]
        [SerializeField] private int m_count = 10;

		[Header( "Reactions" )]
		[SerializeField, Range( 0, 2 )] private float m_strength = 1;
		[SerializeField] private float m_noiseOffset = 1;
		[CinemachineImpulseChannelProperty] 
		[SerializeField] private int m_channelMask;
		[SerializeField] private bool m_use2DDistance = true;
		[SerializeField] private NoiseSettings m_noise = default;
		[SerializeField] private Transform m_listenerSource = default;

        private IPlacement m_placement;
        private IFactory<GameObject> m_spectatorFactory;
		private List<GameObject> m_spectators;

		private void Update()
		{
			var impulseManager = CinemachineImpulseManager.Instance;

			for ( int idx = 0; idx < m_spectators.Count; ++idx )
			{
				GameObject viewer = m_spectators[idx];
				bool hasSignal = impulseManager.GetImpulseAt( viewer.transform.position, m_use2DDistance, m_channelMask, out var localPos, out var localRot );
				if ( hasSignal )
				{
					Transform viewerSprite = viewer.transform.GetChild( 0 );
					viewerSprite.localPosition = localPos + Vector3.up * 2;
					viewerSprite.localRotation = localRot;
				}
			}

			//float time = Time.timeSinceLevelLoad;
			//for ( int idx = 0; idx < m_spectators.Count; ++idx )
			//{
			//	float timeOffset = (idx + 1) * m_noiseOffset;
			//	m_noise.GetSignal( time + timeOffset, out var localPos, out var localRot );

			//	GameObject viewer = m_spectators[idx];
			//	Transform viewerSprite = viewer.transform.GetChild( 0 );
			//	viewerSprite.localPosition = localPos + Vector3.up * 2;
			//	viewerSprite.localRotation = localRot;
			//}
		}

		private void Start()
		{
			m_spectators = new List<GameObject>( m_count );
			for ( int idx = 0; idx < m_count; ++idx )
			{
				m_placement.GetNextOrientation( idx, m_count, out Vector3 spawnPos, out Quaternion spawnRot );
				GameObject spectator = m_spectatorFactory.Create( spawnPos, spawnRot, transform );
				spectator.SetActive( true );

				m_spectators.Add( spectator );
			}
		}

		private void Awake()
		{
            m_placement = GetComponentInChildren<IPlacement>();
            m_spectatorFactory = GetComponentInChildren<IFactory<GameObject>>();
		}
	}
}
