using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;
using Xam.Utility.Extensions;

namespace DashSlash.Vfx.Audiences
{
    public class AudienceReactionFactory : SingletonMono<AudienceReactionFactory>
    {
        [Header( "Reactions" )]
        [SerializeField] private bool m_canReact = true;

        [Space]
        [SerializeField] private AudienceReaction m_excitedReaction = default;
        [SerializeField] private AudienceReaction m_scaredReaction = default;

        public void SetReactionsActive( bool isActive )
		{
            m_canReact = isActive;
		}

        public void PlayExcitedReaction( Vector3 position, Vector3 velocity )
		{
            PlayReaction( position, velocity, m_excitedReaction );
        }

        public void PlayScaredReaction( Vector3 position, Vector3 velocity )
        {
            PlayReaction( position, velocity, m_scaredReaction );
        }

        private void PlayReaction( Vector3 position, Vector3 velocity, AudienceReaction reaction )
		{
            if ( !m_canReact ) { return; }

            reaction.React( position, velocity );
		}
    }
}
