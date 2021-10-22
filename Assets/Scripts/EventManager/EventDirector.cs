using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.EventQueues
{
    public class EventDirector : SerializedMonoBehaviour, IEvent
    {
        public PlayState State { get; private set; } = PlayState.Sleeping;

        private bool IsPlaying => m_queueRoutine != null;

		[ListDrawerSettings( ShowIndexLabels = true )]
        [SerializeField] private List<IEvent> m_events = new List<IEvent>();

        private int m_nextEventIndex = 0;
        private IEvent m_currentEvent;
        private Coroutine m_queueRoutine;
        private WaitForFixedUpdate m_waitForFixedUpdate = new WaitForFixedUpdate();

        public void SetEventIndex( int index )
		{
            m_nextEventIndex = index;
		}

        [ContextMenu( "Reset Queue" )]
        public void ResetQueue()
		{
            Stop();

            m_nextEventIndex = 0;
            m_currentEvent = null;
            State = PlayState.Sleeping;
        }

        [ContextMenu( "Stop" )]
        public void Stop()
        {
            if ( IsPlaying )
            {
                this.TryStopCoroutine( ref m_queueRoutine );
                State = PlayState.Sleeping;
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

            this.Log( $"Start", Colors.Red );

            State = PlayState.Playing;
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
            m_currentEvent = null;
            State = PlayState.Done;

            this.Log( $"Finish", Colors.Red );
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
                State = PlayState.Sleeping;

                this.Log( $"Queue has been interrupted." );
            }
        }
	}
}
