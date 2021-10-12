using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
    using Movement;

    public class Cruiser : Enemy
    {
		private CharacterMotor m_characterMotor;
		private WaveMovement m_waveMovement;

		protected override void UpdateState()
		{
			m_characterMotor.SetDesiredVelocity( FacingDirection );
		}

		protected override void OnAwokenFromSpawn()
		{
			base.OnAwokenFromSpawn();

			UpdateRotation();

			m_waveMovement.enabled = true;
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_characterMotor = GetComponent<CharacterMotor>();
			m_waveMovement = GetComponentInChildren<WaveMovement>( true );
		}
	}
}
