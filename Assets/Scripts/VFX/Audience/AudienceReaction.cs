using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;
using Xam.Utility.Patterns;
using Xam.Utility.Extensions;

namespace DashSlash.Vfx.Audiences
{
    public class AudienceReaction : MonoBehaviour
    {
		private CinemachineImpulseManager ImpulseManager => CinemachineImpulseManager.Instance;

        [CinemachineImpulseDefinitionProperty]
        [SerializeField] private CinemachineImpulseDefinition m_impulseDefinition = new CinemachineImpulseDefinition();

		/// <summary>
		/// </summary>
		/// <param name="velocity"></param>
		/// <returns>Duration of reaction.</returns>
		public float React( Vector3 velocity )
		{
			var allSpectators = DynamicPool.Instance.GetPooledObjectsByType<AudienceSpectator>();
			if ( allSpectators == null ) { return 0; }

			CinemachineImpulseManager.ImpulseEvent lastImpulse = null;
			foreach ( var spectator in allSpectators )
			{
				var impulse = CreateImpulseEvent( velocity );
				spectator.React( impulse );

				lastImpulse = impulse;
			}

			return lastImpulse.m_Envelope.Duration;
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


#if UNITY_EDITOR
		[Header( "Editor / Tools" )]
		[SerializeField] private Color m_radiusColor = Color.cyan;
		[SerializeField] private int m_radiusResolution = 20;

		private void OnDrawGizmosSelected()
		{
			Vector3 center = transform.position;
			Vector3 normal = transform.forward;

			UnityEditor.Handles.color = m_radiusColor;
			UnityEditor.Handles.DrawSolidDisc( center, normal, m_impulseDefinition.m_ImpactRadius );

			for ( int idx = 0; idx < m_radiusResolution; ++idx )
			{
				float radiusScale = idx / (float)(m_radiusResolution - 1);
				float radius = Mathf.Lerp( m_impulseDefinition.m_ImpactRadius, m_impulseDefinition.m_DissipationDistance, radiusScale );

				UnityEditor.Handles.color = m_radiusColor.NewAlpha( DistanceDecay( radius ) );
				UnityEditor.Handles.DrawWireDisc( center, normal, radius );
			}

			UnityEditor.Handles.color = m_radiusColor.NewAlpha( 1 );
			UnityEditor.Handles.DrawWireDisc( center, normal, m_impulseDefinition.m_DissipationDistance );
		}

		/// <summary>
		/// Essentially a copy-paste of <see cref="CinemachineImpulseManager.ImpulseEvent.DistanceDecay(float)"/>.
		/// </summary>
		/// <param name="distance"></param>
		/// <returns></returns>
		public float DistanceDecay( float distance )
		{
			float dissipationDistance = m_impulseDefinition.m_DissipationDistance;

			float radius = Mathf.Max( m_impulseDefinition.m_ImpactRadius, 0 );
			if ( distance < radius )
				return 1;
			distance -= radius;
			if ( distance >= dissipationDistance )
				return 0;

			switch ( m_impulseDefinition.m_DissipationMode )
			{
				default:
				case CinemachineImpulseManager.ImpulseEvent.DissipationMode.LinearDecay:
					return Mathf.Lerp( 1, 0, distance / dissipationDistance );
				case CinemachineImpulseManager.ImpulseEvent.DissipationMode.SoftDecay:
					return 0.5f * (1 + Mathf.Cos( Mathf.PI * (distance / dissipationDistance) ));
				case CinemachineImpulseManager.ImpulseEvent.DissipationMode.ExponentialDecay:
					return 1 - Damper.Damp( 1, dissipationDistance, distance );
			}
		}
#endif
	}
}
