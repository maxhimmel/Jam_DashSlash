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
        [SerializeField] private AudienceReaction m_excitedReactionPrefab = default;
        [SerializeField] private AudienceReaction m_scaredReactionPrefab = default;

        public void SetReactionsActive( bool isActive )
		{
            m_canReact = isActive;
		}

        public void PlayExcitedReaction( Vector3 position, Vector3 velocity )
		{
            PlayReaction( position, velocity, m_excitedReactionPrefab );
        }

        public void PlayScaredReaction( Vector3 position, Vector3 velocity )
        {
            PlayReaction( position, velocity, m_scaredReactionPrefab );
        }

        private void PlayReaction( Vector3 position, Vector3 velocity, AudienceReaction prefab )
		{
            if ( !m_canReact ) { return; }

            var reaction = CreateReaction( position, prefab );
            float duration = reaction.React( velocity );

            Destroy( reaction.gameObject, duration );
		}

        private AudienceReaction CreateReaction( Vector3 position, AudienceReaction prefab )
		{
            return Instantiate( prefab, position, Quaternion.identity );
		}
    }
}
