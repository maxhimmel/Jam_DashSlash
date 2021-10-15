using DashSlash.Gameplay.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay.Patterns;
using Xam.Utility;

namespace DashSlash.Gameplay.Enemies.Utility
{
    public class EnemyWaveMovementConfig : EnemySpawnConfig
    {
		[Tooltip( "This will include disabled components in it's search." )]
		[SerializeField] private bool m_queryDisabledMovement = true;
        [SerializeField] private WaveDatum m_data = new WaveDatum( 2, 2.25f, 0 );

		protected override void OnEnemyCreated( object sender, Enemy e )
		{
			var movement = e.GetComponentInChildren<WaveMovement>( m_queryDisabledMovement );
			if ( movement != null )
			{
				movement.SetWaveData( m_data );
			}
		}
	}
}
