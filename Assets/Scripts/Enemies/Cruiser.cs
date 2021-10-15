using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;

namespace DashSlash.Gameplay.Enemies
{
    using Movement;

    public class Cruiser : Enemy
    {
		private CharacterMotor m_characterMotor;
		private WaveMovement m_waveMovement;
		private ILookRotation m_lookRotation;

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

		protected override Quaternion GetDesiredRotation()
		{
			if ( m_lookRotation != null )
			{
				return m_lookRotation.GetLookRotation( GetDirectionToPlayer(), Vector3.up );
			}

			return base.GetDesiredRotation();
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_characterMotor = GetComponent<CharacterMotor>();
			m_waveMovement = GetComponentInChildren<WaveMovement>( true );
			m_lookRotation = GetComponent<ILookRotation>();
		}
	}
}
