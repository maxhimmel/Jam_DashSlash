using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Utility
{
	public class DirectionOffsetter : MonoBehaviour, IDirection
	{
		[SerializeField] private RandomFloatRange m_offsetAngleRange = new RandomFloatRange( 0, 8 );

		private List<IDirection> m_childDirections = new List<IDirection>();

		Vector3 IDirection.GetDirection( int index, int count )
		{
			Vector3 sum = Vector3.zero;

			foreach ( var direction in m_childDirections )
			{
				sum += direction.GetDirection( index, count );
			}

			return GetOffsetDirection( index, sum );
		}

		private Vector3 GetOffsetDirection( int index, Vector3 direction )
		{
			int offsetDir = (index % 2 == 0) ? 1 : -1;
			float offsetAngle = offsetDir * m_offsetAngleRange.Evaluate();

			return Quaternion.AngleAxis( offsetAngle, Vector3.forward ) * direction;
		}

		private void Awake()
		{
			CacheChildDirections();
		}

		private void CacheChildDirections()
		{
			foreach ( Transform child in transform )
			{
				IDirection direction = child.GetComponent<IDirection>();
				if ( direction != null )
				{
					m_childDirections.Add( direction );
				}
			}
		}
	}
}
