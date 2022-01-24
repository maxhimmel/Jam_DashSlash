using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
    public class Swarmer : DirectionMover
    {
        [Header( "Swarmer" )]
        [SerializeField] private Angry m_angry = new Angry();

		protected override void OnAwokenFromSpawn()
		{
			base.OnAwokenFromSpawn();

            m_body.freezeRotation = true;
		}

		public void SetAngry()
        {
            m_spawnAwakeDelay = m_angry.AwakeDelay;
            m_spawnInvincibiltyDuration = m_angry.SpawnInvincibilityDuration;

            m_rotationSpeed = m_angry.TurnSpeed;

            m_motor.SetMaxSpeed( m_angry.MaxSpeed );
            m_motor.SetAcceleration( m_angry.Acceleration );

            BeginSpawning();
		}

        [System.Serializable]
		public class Angry
        {
            public float MaxSpeed => m_maxSpeed;
            public float Acceleration => m_acceleration;
            public float TurnSpeed => m_turnSpeed;
			public float AwakeDelay => m_awakeDelay;
            public float SpawnInvincibilityDuration => m_spawnInvincibiltyDuration;

			[SerializeField] private float m_maxSpeed = 12;
            [SerializeField] private float m_acceleration = 35;
            [SerializeField] private float m_turnSpeed = 540;

            [Space]
            [SerializeField] private float m_awakeDelay = 0.25f;
            [SerializeField] private float m_spawnInvincibiltyDuration = 0.25f;
        }
	}
}
