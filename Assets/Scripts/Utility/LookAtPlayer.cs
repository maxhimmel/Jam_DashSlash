using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace DashSlash.Gameplay.Player
{
    public class LookAtPlayer : MonoBehaviour
    {
		private PlayerController Player => DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();

		private void Update()
		{
			transform.rotation = Quaternion.LookRotation( Vector3.forward, GetDirection() );
		}

		private Vector3 GetDirection()
		{
			return (Player.transform.position - transform.position).normalized;
		}
	}
}
