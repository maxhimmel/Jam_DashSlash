using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.EventQueues
{
	public class HaltEvent : IEvent
	{
		public PlayState State => PlayState.Playing;

		public void Play()
		{
			Debug.LogWarning( $"Halt Event played. <b>Event Director will not continue</b>" );
		}
	}
}
