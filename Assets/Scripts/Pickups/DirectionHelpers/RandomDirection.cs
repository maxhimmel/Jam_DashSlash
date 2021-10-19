using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Utility
{
	public class RandomDirection : MonoBehaviour, IDirection
	{
		public Vector3 GetDirection( int index, int count )
		{
			return Random.insideUnitCircle.normalized;
		}
	}
}
