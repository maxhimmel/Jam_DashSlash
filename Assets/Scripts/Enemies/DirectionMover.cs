using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
    using Movement;

    public class DirectionMover : Enemy
    {
        private CharacterMotor m_motor;

		protected override void UpdateState()
		{
			base.UpdateState();

			m_motor.SetDesiredVelocity( FacingDirection );
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_motor = GetComponent<CharacterMotor>();
		}
	}
}
