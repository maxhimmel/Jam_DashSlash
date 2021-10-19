using DashSlash.Gameplay.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Utility
{
	public class PlayerDirection : MonoBehaviour, IDirection
	{
		[SerializeField] private bool m_isInverted = false;
		[SerializeField] private RandomFloatRange m_offsetAngleRange = new RandomFloatRange( 0, 8 );

		private LookAtPlayer m_lookAtPlayer = new LookAtPlayer();

		public Vector3 GetDirection( int index, int count )
		{
			int offsetDir = (index % 2 == 0) ? 1 : -1;
			float offsetAngle = offsetDir * m_offsetAngleRange.Evaluate();

			Vector3 dirToPlayer = m_lookAtPlayer.GetDirection( transform.position );
			if ( m_isInverted )
			{
				dirToPlayer *= -1;
			}

			return Quaternion.AngleAxis( offsetAngle, Vector3.forward ) * dirToPlayer;
		}
	}
}
