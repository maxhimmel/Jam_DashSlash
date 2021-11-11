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

		[CinemachineImpulseDefinitionProperty]
		[SerializeField] private CinemachineImpulseDefinition m_impulseDefinition = new CinemachineImpulseDefinition();

		private IPlacement m_placement;
        private IFactory<GameObject> m_spectatorFactory;
		private List<GameObject> m_spectators;
		private Dictionary<int, CinemachineImpulseManager.ImpulseEvent> m_impulses = new Dictionary<int, CinemachineImpulseManager.ImpulseEvent>();

		private void Update()
		{
			var impulseManager = CinemachineImpulseManager.Instance;


			// 3)
			if ( Input.GetKeyDown( KeyCode.Backspace ) )
			{
				for ( int idx = 0; idx < m_spectators.Count; ++idx )
				{
					var velocity = Random.insideUnitCircle;

					var impulse = impulseManager.NewImpulseEvent();
					impulse.m_Channel = m_impulseDefinition.m_ImpulseChannel;
					impulse.m_DirectionMode = m_impulseDefinition.m_DirectionMode;
					impulse.m_DissipationDistance = m_impulseDefinition.m_DissipationDistance;
					impulse.m_DissipationMode = m_impulseDefinition.m_DissipationMode;
					impulse.m_Envelope = m_impulseDefinition.m_TimeEnvelope;
					impulse.m_Position = m_listenerSource.position;
					impulse.m_PropagationSpeed = m_impulseDefinition.m_PropagationSpeed;
					impulse.m_Radius = m_impulseDefinition.m_ImpactRadius;
					impulse.m_SignalSource = new SignalSource( m_impulseDefinition, velocity );
					impulse.m_StartTime = impulseManager.CurrentTime;

					m_impulses[idx] = impulse;
				}
			}
			for ( int idx = 0; idx < m_spectators.Count; ++idx )
			{
				GameObject viewer = m_spectators[idx];

				if ( !m_impulses.TryGetValue( idx, out var impulse ) ) { continue; }

				if ( impulse.Expired )
				{
					m_impulses.Remove( idx );
					continue;
				}

				bool hasSignal = impulse.GetDecayedSignal( viewer.transform.position, m_use2DDistance, out var localPos, out var localRot );
				if ( !hasSignal ) { continue; }

				Transform viewerSprite = viewer.transform.GetChild( 0 );
				viewerSprite.localPosition = localPos + Vector3.up * 2;
				viewerSprite.localRotation = localRot;
			}


			// 2) 
			//for ( int idx = 0; idx < m_spectators.Count; ++idx )
			//{
			//	GameObject viewer = m_spectators[idx];
			//	bool hasSignal = impulseManager.GetImpulseAt( viewer.transform.position, m_use2DDistance, m_channelMask, out var localPos, out var localRot );
			//	if ( hasSignal )
			//	{
			//		Transform viewerSprite = viewer.transform.GetChild( 0 );
			//		viewerSprite.localPosition = localPos + Vector3.up * 2;
			//		viewerSprite.localRotation = localRot;
			//	}
			//}


			// 1)
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

		class SignalSource : ISignalSource6D
		{
			private CinemachineImpulseDefinition m_def;
			private Vector3 m_velocity;
			private float m_startTimeOffset = 0;

			public SignalSource( CinemachineImpulseDefinition def, Vector3 velocity )
			{
				m_def = def;
				m_velocity = velocity;
				if ( m_def.m_Randomize && m_def.m_RawSignal.SignalDuration <= 0 )
					m_startTimeOffset = UnityEngine.Random.Range( -1000f, 1000f );
			}

			public float SignalDuration { get { return m_def.m_RawSignal.SignalDuration; } }

			public void GetSignal( float timeSinceSignalStart, out Vector3 pos, out Quaternion rot )
			{
				float time = m_startTimeOffset + timeSinceSignalStart * m_def.m_FrequencyGain;

				// Do we have to fit the signal into the envelope?
				float signalDuration = SignalDuration;
				if ( signalDuration > 0 )
				{
					if ( m_def.m_RepeatMode == CinemachineImpulseDefinition.RepeatMode.Loop )
						time %= signalDuration;
					else if ( m_def.m_TimeEnvelope.Duration > Cinemachine.Utility.UnityVectorExtensions.Epsilon )
						time *= m_def.m_TimeEnvelope.Duration / signalDuration; // stretch
				}

				m_def.m_RawSignal.GetSignal( time, out pos, out rot );
				float gain = m_velocity.magnitude;
				gain *= m_def.m_AmplitudeGain;
				pos *= gain;
				pos = Quaternion.FromToRotation( Vector3.down, m_velocity ) * pos;
				rot = Quaternion.SlerpUnclamped( Quaternion.identity, rot, gain );
			}
		}
	}
}
