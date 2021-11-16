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

			if ( CanUpdateSegments() )
			{
				UpdateSegmentMovement();
			}
		}

		private bool CanUpdateSegments()
		{
			return m_segments != null && m_segments.Count > 1;
		}

		private void UpdateSegmentMovement()
		{
			// We start from 1 to skip this head segment ...
			for ( int idx = 1; idx < m_segments.Count; ++idx )
			{
				var segment = m_segments[idx];
				segment.UpdateFollowMovement();
			}
		}

		private void OnSegmentSliced( object sender, EventArgs e )
		{
			var slicedComponent = sender as Component;
			var slicedSegment = slicedComponent?.GetComponent<SnakeSegment>();
			Debug.Assert( slicedSegment != null, $"Sliced segment must be of type 'SnakeSegment.'", this );

			int newHeadIndex = GetNewHeadIndex( slicedSegment );

			TrySpawnLoot( newHeadIndex );

			if ( TryHandleTailSegment( newHeadIndex ) ) 
			{ 
				return; 
			}

			var discardedSegments = PopDiscardedSegments( newHeadIndex );

			CreateNewSnakeHead( discardedSegments );
		}

		private void TrySpawnLoot( int newHeadIndex )
		{
			if ( newHeadIndex <= 1 ) 
			{ 
				return; 
			}

			var slicedSegment = m_segments[newHeadIndex - 1];
			m_lootSpawner.Spawn( slicedSegment.transform.position );
		}

		private int GetNewHeadIndex( SnakeSegment slicedSegment )
		{
			for ( int idx = 0; idx < m_segments.Count; ++idx )
			{
				var segment = m_segments[idx];
				if ( segment != slicedSegment ) { continue; }

				return idx + 1;
			}

			return -1;
		}

		private bool TryHandleTailSegment( int newHeadIndex )
		{
			if ( newHeadIndex >= m_segments.Count )
			{
				int tailIndice = m_segments.Count - 1;
				m_segments.RemoveAt( tailIndice );
				return true;
			}

			return false;
		}

		private List<SnakeSegment> PopDiscardedSegments( int newHeadIndex )
		{
			int segmentCount = m_segments.Count - newHeadIndex;

			var discardedSegments = m_segments.GetRange( newHeadIndex, segmentCount );
			m_segments.RemoveRange( newHeadIndex - 1, segmentCount + 1 );

			foreach ( var segment in discardedSegments )
			{
				segment.Sliceable.Sliced -= OnSegmentSliced;
			}

			return discardedSegments;
		}

		private SnakeHead CreateNewSnakeHead( List<SnakeSegment> segments )
		{
			var newHeadSegment = segments[0];
			var newHead = newHeadSegment.gameObject.AddComponent<SnakeHead>();

			newHead.SetSegments( segments );

			return newHead;
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
