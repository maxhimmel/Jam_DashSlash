using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Player
{
    public partial class PlayerController
	{

		[System.Serializable]
		class DebugCheats
		{
			[SerializeField] private int m_pickupColections = 10;

			[Space]
			[SerializeField] private KeyCode m_collectPickupsKey = KeyCode.Space;
			[SerializeField] private KeyCode m_takeDamageKey = KeyCode.Return;
			[SerializeField] private KeyCode m_excitedReactionKey = KeyCode.Backspace;

			public void Update( PlayerController player )
			{
				if ( Input.GetKeyDown( m_collectPickupsKey ) )
				{
					ICollector self = player;
					for ( int idx = 0; idx < m_pickupColections; ++idx )
					{
						self.Collect( null );
					}
				}

				if ( Input.GetKeyDown( m_takeDamageKey ) )
				{
					IDamageable damaegable = player.GetComponentInChildren<IDamageable>();
					damaegable.TakeDamage( new DamageDatum()
					{
						DamageCauser = player.transform,
						Instigator = player.transform
					} );
				}

				if ( Input.GetKeyDown( m_excitedReactionKey ) )
				{
					Vfx.Audiences.AudienceReactionFactory.Instance.PlayExcitedReaction( player.transform.position, Vector3.up );
				}
			}
		}

	}
}
