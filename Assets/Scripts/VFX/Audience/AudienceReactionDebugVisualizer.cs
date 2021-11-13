using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;
using Xam.Utility.Extensions;

namespace DashSlash.Vfx.Audiences
{
    public class AudienceReactionDebugVisualizer : MonoBehaviour
    {
#if UNITY_EDITOR
		private CinemachineImpulseDefinition ReactionImpulse => m_reaction.ImpulseDefinition;

		[Header( "Editor / Tools" )]
		[SerializeField] private AudienceReaction m_reaction = default;

		[Space]
		[SerializeField] private Color m_radiusColor = Color.cyan;
		[SerializeField] private int m_radiusResolution = 20;

		private void OnValidate()
		{
			if ( m_reaction == null )
			{
				Debug.LogWarning( $"Missing Reaction reference on visualizer ('{name}')", this );
			}
		}

		private void OnDrawGizmosSelected()
		{
			if ( m_reaction == null ) { return; }

			Vector3 center = transform.position;
			Vector3 normal = transform.forward;

			UnityEditor.Handles.color = m_radiusColor;
			UnityEditor.Handles.DrawSolidDisc( center, normal, ReactionImpulse.m_ImpactRadius );

			for ( int idx = 0; idx < m_radiusResolution; ++idx )
			{
				float radiusScale = idx / (float)(m_radiusResolution - 1);
				float radius = Mathf.Lerp( ReactionImpulse.m_ImpactRadius, ReactionImpulse.m_DissipationDistance, radiusScale );

				UnityEditor.Handles.color = m_radiusColor.NewAlpha( DistanceDecay( radius ) );
				UnityEditor.Handles.DrawWireDisc( center, normal, radius );
			}

			UnityEditor.Handles.color = m_radiusColor.NewAlpha( 1 );
			UnityEditor.Handles.DrawWireDisc( center, normal, ReactionImpulse.m_DissipationDistance );
		}

		/// <summary>
		/// Essentially a copy-paste of <see cref="CinemachineImpulseManager.ImpulseEvent.DistanceDecay(float)"/>.
		/// </summary>
		/// <param name="distance"></param>
		/// <returns></returns>
		public float DistanceDecay( float distance )
		{
			float dissipationDistance = ReactionImpulse.m_DissipationDistance;

			float radius = Mathf.Max( ReactionImpulse.m_ImpactRadius, 0 );
			if ( distance < radius )
				return 1;
			distance -= radius;
			if ( distance >= dissipationDistance )
				return 0;

			switch ( ReactionImpulse.m_DissipationMode )
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
