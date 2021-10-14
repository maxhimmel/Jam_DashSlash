using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Initialization;

namespace DashSlash.Gameplay.EventQueues
{
	[RequireComponent( typeof( EventDirector ) )]
    public class DirectorKickOff : MonoBehaviour
    {
		[SerializeField, Min( 0 )] private int m_startIndex = 0;

		private IEnumerator Start()
		{
			while ( !LevelInitializer.IsInitialized ) { yield return new WaitForFixedUpdate(); }

			EventDirector director = GetComponent<EventDirector>();
			director.SetEventIndex( m_startIndex );
			director.Play();
		}
	}
}
