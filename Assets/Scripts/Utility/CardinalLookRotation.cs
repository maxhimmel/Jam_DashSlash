using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;

namespace DashSlash.Utility
{
	public class CardinalLookRotation : MonoBehaviour, ILookRotation
	{
		private const int k_cardinalCount = 4;
		private readonly Vector3[] k_cardinals = new Vector3[k_cardinalCount]
		{
			Vector3.up,
			Vector3.down,
			Vector3.left,
			Vector3.right
		};

		[SerializeField] private CardinalMode m_cardinalMode = CardinalMode.Full;

		public Quaternion GetLookRotation( Vector3 direction, Vector3 up )
		{
			Vector3 alignedCardinal = Vector3.one * -1;
			float highestDot = Mathf.NegativeInfinity;

			foreach ( var cardinalDirection in GetCardinalDirections() )
			{
				float dot = Vector3.Dot( cardinalDirection, direction );

				if ( dot > highestDot )
				{
					highestDot = dot;
					alignedCardinal = cardinalDirection;
				}
			}

			return Quaternion.LookRotation( Vector3.forward, alignedCardinal );
		}

		private IEnumerable<Vector3> GetCardinalDirections()
		{
			switch ( m_cardinalMode )
			{
				case CardinalMode.Full:
					for ( int idx = 0; idx < k_cardinalCount; ++idx )
					{
						yield return k_cardinals[idx];
					}
					break;

				case CardinalMode.Vertical:
					for ( int idx = 0; idx < 2; ++idx )
					{
						yield return k_cardinals[idx];
					}
					break;

				case CardinalMode.Horizontal:
					for ( int idx = 2; idx < k_cardinalCount; ++idx )
					{
						yield return k_cardinals[idx];
					}
					break;
			}
		}

		private enum CardinalMode
		{
			Full = Vertical | Horizontal,

			Vertical = 1 << 0,
			Horizontal = 1 << 1
		}
	}
}
