using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using Xam.Utility;

namespace DashSlash.Gameplay.Enemies
{
	using Slicing;

	public class SnakeHead : Enemy
    {
        [Header( "Snake" )]
        [SerializeField] private int m_segmentCount = 5;
		[SerializeField] private SnakeSegment m_segmentPrefab = default;

		private LazyCachedChildComponent<Rig> m_rig = new LazyCachedChildComponent<Rig>( false );
		private LazyCachedChildComponent<RigBuilder> m_rigBuilder = new LazyCachedChildComponent<RigBuilder>( false );
		private LazyCachedChildComponent<SnakeSegment> m_headSegment = new LazyCachedChildComponent<SnakeSegment>( false );
		private List<SnakeSegment> m_segments = new List<SnakeSegment>();
		private bool m_isOriginalSnake = true;

		protected override void InitReferences()
		{
			base.InitReferences();

			if ( m_isOriginalSnake )
			{
				CreateSnakeSegments();
				CreateRigSegments();

				PostProcessSegments();
			}
		}

		private void CreateSnakeSegments()
		{
			SnakeSegment prevSegment = m_headSegment[this];

			m_segments.Capacity = m_segmentCount;
			for ( int idx = 0; idx < m_segmentCount; ++idx )
			{
				SnakeSegment newSegment = CreateSnakeSegment( prevSegment );
				prevSegment = newSegment;

				m_segments.Add( newSegment );
			}
		}

		private SnakeSegment CreateSnakeSegment( SnakeSegment prevSegment )
		{
			Quaternion parentRot = prevSegment.Segment.rotation;
			Vector3 parentPos = prevSegment.Segment.position;
			Vector3 offset = parentRot * Vector3.down * prevSegment.Radius * 2;
			Vector3 spawnPos = parentPos + offset;

			SnakeSegment newSegment = Instantiate( m_segmentPrefab, spawnPos, parentRot, prevSegment.Segment );
			newSegment.gameObject.SetActive( true );

			newSegment.name = newSegment.name.Replace( "Clone", $" {m_segments.Count} " );

			return newSegment;
		}

		private void CreateRigSegments()
		{
			SnakeSegment prevSegment = m_headSegment[this];

			for ( int idx = 0; idx < m_segmentCount; ++idx )
			{
				SnakeSegment nextSegment = m_segments[idx];
				CreateRigSegment( prevSegment, nextSegment );

				prevSegment = nextSegment;
			}

			m_rigBuilder[this].Build();
		}

		private DampedTransform CreateRigSegment( SnakeSegment prevSegment, SnakeSegment newSegment )
		{
			GameObject segment = new GameObject( "DampedSegment" );
			segment.transform.SetParent( m_rig[this].transform, false );

			DampedTransform dampedSegment = segment.AddComponent<DampedTransform>();
			dampedSegment.data.constrainedObject = newSegment.Segment;
			dampedSegment.data.sourceObject = prevSegment.Segment;
			dampedSegment.data.dampPosition = newSegment.DampPosition;
			dampedSegment.data.dampRotation = newSegment.DampRotation;
			dampedSegment.data.maintainAim = newSegment.MaintainAim;

			return dampedSegment;
		}

		private void PostProcessSegments()
		{
			for ( int idx = 0; idx < m_segments.Count; ++idx )
			{
				SnakeSegment segment = m_segments[idx];
				ISliceable sliceable = segment.GetComponent<ISliceable>();

				sliceable.Sliced += OnSegmentSliced;
			}
		}

		private void OnSegmentSliced( object sender, System.EventArgs e )
		{
			Component slicedComponent = sender as Component;
			SnakeSegment slicedSegment = slicedComponent?.GetComponent<SnakeSegment>();
			Debug.Assert( slicedSegment != null, $"Sliced segment must be of type 'SnakeSegment.'", this );

			if ( !TryOrganizeSnakeSplit( slicedSegment, out var nextHeadSegment, out var splitSegments ) ) { return; }

			nextHeadSegment.gameObject.SetActive( false );

			foreach ( var segment in splitSegments )
			{
				ISliceable sliceable = segment.GetComponent<ISliceable>();
				sliceable.Sliced -= OnSegmentSliced;
			}

			SnakeHead newHead = CreateSlicedSnake( nextHeadSegment );
			newHead.CreateRigSegments();
			newHead.PostProcessSegments();
		}

		private bool TryOrganizeSnakeSplit( SnakeSegment slicedSegment, out SnakeSegment nextHeadSegment, out List<SnakeSegment> splitSegments )
		{
			splitSegments = null;
			nextHeadSegment = null;

			for ( int idx = 0; idx < m_segments.Count - 1; ++idx )
			{
				SnakeSegment otherSegment = m_segments[idx];
				if ( otherSegment != slicedSegment ) { continue; }

				int nextSegmentIndex = idx + 1;
				nextHeadSegment = m_segments[nextSegmentIndex];

				splitSegments = m_segments.GetRange( nextSegmentIndex, m_segments.Count - nextSegmentIndex );
				m_segments.RemoveRange( idx, m_segments.Count - idx );

				return true;
			}

			return false;
		}

		private SnakeHead CreateSlicedSnake( SnakeSegment headSegmentPrefab )
		{
			SnakeSegment newHeadSegment = Instantiate( headSegmentPrefab, headSegmentPrefab.transform.position, headSegmentPrefab.transform.rotation );
			newHeadSegment.gameObject.SetActive( true );

			List<SnakeSegment> newSegments = new List<SnakeSegment>();
			newHeadSegment.GetComponentsInChildren( false, newSegments );
			newSegments.Remove( newHeadSegment );

			SnakeHead newHead = newHeadSegment.gameObject.AddComponent<SnakeHead>();
			newHead.m_isOriginalSnake = false;
			newHead.m_segmentCount = newSegments.Count;
			newHead.m_segments = newSegments;

			newHead.m_body.bodyType = RigidbodyType2D.Dynamic;
			newHead.name = newHead.name.Replace( "Segment", "NewHead" );

			return newHead;
		}
	}
}
