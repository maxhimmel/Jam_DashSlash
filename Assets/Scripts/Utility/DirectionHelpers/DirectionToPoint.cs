using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Utility
{
	public class DirectionToPoint : MonoBehaviour, IDirection
	{
		[SerializeField] private Vector3 m_worldPosition = Vector3.zero;

		Vector3 IDirection.GetDirection( int index, int count )
		{
			return (m_worldPosition - transform.position).normalized;
		}
	}
}
