using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;
using Xam.Utility.Patterns;

namespace DashSlash.Vfx.Audiences
{
    public class AudienceReaction : MonoBehaviour
    {
		private CinemachineImpulseManager ImpulseManager => CinemachineImpulseManager.Instance;

        [CinemachineImpulseDefinitionProperty]
        [SerializeField] private CinemachineImpulseDefinition m_impulseDefinition = new CinemachineImpulseDefinition();

		public void React( Vector3 velocity )
		{
			var allSpectators = DynamicPool.Instance.GetPooledObjectsByType<AudienceSpectator>();
			if ( allSpectators == null ) { return; }

			foreach ( var spectator in allSpectators )
			{
				var impulse = CreateImpulseEvent( velocity );
				spectator.React( impulse );
			}
		}

		private CinemachineImpulseManager.ImpulseEvent CreateImpulseEvent( Vector3 velocity )
		{
			var impulse = ImpulseManager.NewImpulseEvent();

			impulse.m_Channel = m_impulseDefinition.m_ImpulseChannel;
			impulse.m_DirectionMode = m_impulseDefinition.m_DirectionMode;
			impulse.m_DissipationDistance = m_impulseDefinition.m_DissipationDistance;
			impulse.m_DissipationMode = m_impulseDefinition.m_DissipationMode;
			impulse.m_Envelope = m_impulseDefinition.m_TimeEnvelope;
			impulse.m_Position = transform.position;
			impulse.m_PropagationSpeed = m_impulseDefinition.m_PropagationSpeed;
			impulse.m_Radius = m_impulseDefinition.m_ImpactRadius;
			impulse.m_SignalSource = new SignalSource( m_impulseDefinition, velocity );
			impulse.m_StartTime = ImpulseManager.CurrentTime;

			return impulse;
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
