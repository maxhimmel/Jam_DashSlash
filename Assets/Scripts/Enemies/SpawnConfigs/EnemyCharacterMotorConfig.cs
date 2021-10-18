using DashSlash.Gameplay.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies.Utility
{
	public class EnemyCharacterMotorConfig : EnemySpawnConfig
	{
		[SerializeField] private float m_maxSpeed = 4;
		[SerializeField] private float m_acceleration = 10;

		protected override void OnEnemyCreated( object sender, Enemy e )
		{
			var motor = e.GetComponent<CharacterMotor>();
			if ( motor != null )
			{
				motor.SetMaxSpeed( m_maxSpeed );
				motor.SetAcceleration( m_acceleration );
			}
		}
	}
}
