using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DashSlash.Gameplay.Enemies
{
	using Movement;
	using Factories;

	public class SnakeHead : Enemy
    {
		private SnakeSegmentFactory m_segmentFactory;
		private List<SnakeSegment> m_segments;
		private SnakeSegment m_headSegment;
		private CharacterMotor m_motor;

		protected override void UpdateState()
		{
			base.UpdateState();

			m_motor.SetDesiredVelocity( FacingDirection );

			if ( m_segments != null && m_segments.Count > 1 )
			{
				// We start from 1 to skip this head segment ...
				for ( int idx = 1; idx < m_segments.Count; ++idx )
				{
					var segment = m_segments[idx];
					segment.UpdateFollowMovement();
				}
			}
		}

		private void OnSegmentSliced( object sender, EventArgs e )
		{
			var slicedComponent = sender as Component;
			var slicedSegment = slicedComponent?.GetComponent<SnakeSegment>();
			Debug.Assert( slicedSegment != null, $"Sliced segment must be of type 'SnakeSegment.'", this );

			int newHeadIndice = -1;
			for ( int idx = 0; idx < m_segments.Count; ++idx )
			{
				var segment = m_segments[idx];
				if ( segment != slicedSegment ) { continue; }

				newHeadIndice = idx + 1;
				break;
			}

			if ( newHeadIndice >= m_segments.Count )
			{
				int tailIndice = m_segments.Count - 1;
				m_segments.RemoveAt( tailIndice );
				return;
			}

			int segmentCount = m_segments.Count - newHeadIndice;
			var discardedSegments = m_segments.GetRange( newHeadIndice, segmentCount );
			foreach ( var segment in discardedSegments )
			{
				segment.Sliceable.Sliced -= OnSegmentSliced;
			}

			var newHeadSegment = discardedSegments[0];
			var newHead = newHeadSegment.gameObject.AddComponent<SnakeHead>();
			newHead.SetSegments( discardedSegments );

			m_segments.RemoveRange( newHeadIndice - 1, segmentCount + 1 );
		}

		protected override void InitReferences()
		{
			base.InitReferences();

			if ( m_segmentFactory != null )
			{
				var segments = m_segmentFactory.CreateSegments( m_headSegment );
				SetSegments( segments );
			}
		}

		private void SetSegments( List<SnakeSegment> segments )
		{
			m_segments = segments;

			foreach ( var segment in segments )
			{
				segment.Sliceable.Sliced += OnSegmentSliced;
			}
		}

		protected override void CacheReferences()
		{
			base.CacheReferences();

			m_headSegment = GetComponent<SnakeSegment>();
			m_motor = GetComponent<CharacterMotor>();
			m_segmentFactory = GetComponentInChildren<SnakeSegmentFactory>();
		}
	}
}
