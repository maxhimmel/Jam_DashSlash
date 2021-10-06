using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.EventQueues
{
    public class EventDirector : SerializedMonoBehaviour
    {
        public bool IsPlaying => m_queueRoutine != null;

        [SerializeField] private List<IEvent> m_events = new List<IEvent>();

        private int m_nextEventIndex = 0;
        private IEvent m_currentEvent;
        private Coroutine m_queueRoutine;
        private WaitForFixedUpdate m_waitForFixedUpdate = new WaitForFixedUpdate();

        [ContextMenu( "Reset Queue" )]
        public void ResetQueue()
		{
            Stop();

            m_nextEventIndex = 0;
            m_currentEvent = null;
        }

        [ContextMenu( "Stop" )]
        public void Stop()
        {
            if ( IsPlaying )
            {
                this.TryStopCoroutine( ref m_queueRoutine );
            }
        }

        [ContextMenu( "Play" )]
        public void Play()
		{
            if ( IsPlaying ) { return; }
            if ( m_events.Count <= 0 ) { return; }

            if ( m_nextEventIndex >= m_events.Count )
			{
                Debug.LogWarning( $"Cannot play completed EventDirector: '{name}'. Try calling Reset before Playing again.", this );
                return;
			}

            m_queueRoutine = StartCoroutine( UpdateQueue() );
		}

        private IEnumerator UpdateQueue()
		{
            while ( m_nextEventIndex < m_events.Count )
            {
                m_currentEvent = GetNextEvent();
                if ( m_currentEvent != null )
                {
                    m_currentEvent.Play();
                    while ( m_currentEvent.State != PlayState.Done )
                    {
                        yield return m_waitForFixedUpdate;
                    }
                }
			}

            m_queueRoutine = null;
		}

        private IEvent GetNextEvent()
        {
            IEvent nextEvent = m_events[m_nextEventIndex];
            ++m_nextEventIndex;

            if ( nextEvent == null )
			{
                Debug.LogWarning( $"Null event found at index '{m_nextEventIndex - 1}'.", this );
			}

            return nextEvent;
        }

		private void OnDisable()
		{
            if ( IsPlaying )
            {
                this.TryStopCoroutine( ref m_queueRoutine );
                Debug.Log( $"{name}'s queue has been interrupted.", this );
            }
        }
	}
}
