using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Xam.Utility.Extensions;

namespace DashSlash.Vfx.Audiences
{
    public class AudienceSpectator : MonoBehaviour
    {
		[SerializeField] private float m_reactionStrength = 1;

        private List<CinemachineImpulseManager.ImpulseEvent> m_reactions = new List<CinemachineImpulseManager.ImpulseEvent>();
		private Vector3 m_impulsePosLastFrame;
		private Quaternion m_impulseRotLastFrame;

        public void React( CinemachineImpulseManager.ImpulseEvent impulse )
		{
            m_reactions.Add( impulse );
		}

		/// <summary>
		/// Essentially a copy-paste of <see cref="CinemachineIndependentImpulseListener"/>.
		/// </summary>
		private void Update()
		{
			if ( !CanReact() ) { return; }

			// Unapply previous shake
			transform.position -= m_impulsePosLastFrame;
			transform.rotation = transform.rotation * Quaternion.Inverse( m_impulseRotLastFrame );
		}

		/// <summary>
		/// Essentially a copy-paste of <see cref="CinemachineIndependentImpulseListener"/>.
		/// </summary>
		private void LateUpdate()
		{
			if ( !CanReact() ) { return; }

			if ( TryGetDecayedSignal( out m_impulsePosLastFrame, out m_impulseRotLastFrame ) )
			{
				ApplyReaction( ref m_impulsePosLastFrame, ref m_impulseRotLastFrame );
			}
		}

		private bool CanReact()
		{
			return m_reactions.Count > 0;
		}

		/// <summary>
		/// Essentially a copy-paste of <see cref="CinemachineImpulseManager"/>.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		private bool TryGetDecayedSignal( out Vector3 position, out Quaternion rotation )
		{
			bool hasSignal = false;
			position = Vector3.zero;
			rotation = Quaternion.identity;

			for ( int idx = 0; idx < m_reactions.Count; ++idx )
			{
				var impulse = m_reactions[idx];
				if ( impulse.Expired )
				{
					m_reactions.RemoveAt( idx-- );
					continue;
				}

				if ( impulse.GetDecayedSignal( transform.position, true, out var localPos, out var localRot ) )
				{
					hasSignal = true;
					position += localPos;
					rotation *= localRot;
				}
			}

			return hasSignal;
		}

		/// <summary>
		/// Essentially a copy-paste of <see cref="CinemachineIndependentImpulseListener"/>.
		/// </summary>
		private void ApplyReaction( ref Vector3 impulsePos, ref Quaternion impulseRot )
		{
			impulsePos *= m_reactionStrength;
			impulseRot = Quaternion.SlerpUnclamped(
				Quaternion.identity, impulseRot, -m_reactionStrength );

			transform.position += impulsePos;
			transform.rotation = transform.rotation * impulseRot;
		}
	}
}
