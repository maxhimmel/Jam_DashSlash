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
		private CinemachineImpulseManager ImpulseManager => CinemachineImpulseManager.Instance;

		[Header( "Spawning" )]
        [SerializeField] private int m_count = 10;

		[Header( "Reactions" )]
		[SerializeField] private Transform m_reactionSource = default;
		[CinemachineImpulseDefinitionProperty]
		[SerializeField] private CinemachineImpulseDefinition m_impulseDefinition = new CinemachineImpulseDefinition();

		private IPlacement m_placement;
        private IFactory<GameObject> m_spectatorFactory;
		private List<GameObject> m_spectators;
		private Dictionary<int, CinemachineImpulseManager.ImpulseEvent> m_impulses = new Dictionary<int, CinemachineImpulseManager.ImpulseEvent>();

		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.Backspace ) )
			{
				CreateReaction( m_reactionSource.position );
			}

			for ( int idx = 0; idx < m_spectators.Count; ++idx )
			{
				if ( TryUpdateSignal( idx, out var localPos, out var localRot ) )
				{
					GameObject viewer = m_spectators[idx];
					Transform viewerSprite = viewer.transform.GetChild( 0 );

					viewerSprite.localPosition = localPos + Vector3.up * 2;
					viewerSprite.localRotation = localRot;
				}
			}
		}

		public void CreateReaction( Vector3 reactionPosition )
		{
			for ( int idx = 0; idx < m_spectators.Count; ++idx )
			{
				CreateImpulseEvent( reactionPosition, idx );
			}
		}

		private CinemachineImpulseManager.ImpulseEvent CreateImpulseEvent( Vector3 position, int spectatorIndex )
		{
			var velocity = Random.insideUnitCircle;

			var impulse = ImpulseManager.NewImpulseEvent();
			impulse.m_Channel = m_impulseDefinition.m_ImpulseChannel;
			impulse.m_DirectionMode = m_impulseDefinition.m_DirectionMode;
			impulse.m_DissipationDistance = m_impulseDefinition.m_DissipationDistance;
			impulse.m_DissipationMode = m_impulseDefinition.m_DissipationMode;
			impulse.m_Envelope = m_impulseDefinition.m_TimeEnvelope;
			impulse.m_Position = position;
			impulse.m_PropagationSpeed = m_impulseDefinition.m_PropagationSpeed;
			impulse.m_Radius = m_impulseDefinition.m_ImpactRadius;
			impulse.m_SignalSource = new SignalSource( m_impulseDefinition, velocity );
			impulse.m_StartTime = ImpulseManager.CurrentTime;

			m_impulses[spectatorIndex] = impulse;

			return impulse;
		}

		private bool TryUpdateSignal( int spectatorIndex, out Vector3 pos, out Quaternion rot )
		{
			pos = Vector3.zero;
			rot = Quaternion.identity;

			if ( !m_impulses.TryGetValue( spectatorIndex, out var impulse ) ) { return false; }

			if ( impulse.Expired )
			{
				m_impulses.Remove( spectatorIndex );
				return false;
			}

			GameObject viewer = m_spectators[spectatorIndex];
			return impulse.GetDecayedSignal( viewer.transform.position, use2D: true, out pos, out rot );
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

		/// <summary>
		/// Copy-paste of <see cref="CinemachineImpulseDefinition.SignalSource"/> due to access levels of class.
		/// </summary>
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
					m_startTimeOffset = Random.Range( -1000f, 1000f );
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
