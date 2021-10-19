using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace DashSlash.Gameplay.Player
{
	[System.Serializable]
    public class LookAtPlayer
    {
		private PlayerController Player => DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();

		public Quaternion GetRotation( Vector3 other )
		{
			return Quaternion.LookRotation( Vector3.forward, GetDirection( other ) );
		}

		/// <summary>
		/// Is normalized.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Vector3 GetDirection( Vector3 other )
		{
			return (Player.transform.position - other).normalized;
		}

		public float GetDistance( Vector3 other )
		{
			return (Player.transform.position - other).magnitude;
		}

		public float GetDistanceSqr( Vector3 other )
		{
			return (Player.transform.position - other).sqrMagnitude;
		}
	}
}
