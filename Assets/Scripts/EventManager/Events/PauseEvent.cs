using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.EventQueues
{
	[System.Serializable]
	public class PauseEvent : IEvent
	{
		public PlayState State => GetPlayState();

		[SerializeField, Min( 0 )] private float m_duration = 1;

		private float m_endTime;

		public void Play()
		{
			m_endTime = Time.timeSinceLevelLoad + m_duration;
		}

		private PlayState GetPlayState()
		{
			if ( m_endTime <= 0 ) { return PlayState.Sleeping; }

			return m_endTime > Time.timeSinceLevelLoad
				? PlayState.Playing
				: PlayState.Done;
		}
	}
}
