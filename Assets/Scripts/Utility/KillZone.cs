using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.LevelPieces
{
    public class KillZone : MonoBehaviour
    {
		private void OnTriggerEnter2D( Collider2D collision )
		{
			Rigidbody2D body = collision.attachedRigidbody;
			GameObject root = body != null
				? body.gameObject
				: collision.gameObject;

			Destroy( root );
		}
	}
}
