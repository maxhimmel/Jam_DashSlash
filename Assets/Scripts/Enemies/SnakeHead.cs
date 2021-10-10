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

		protected override void InitReferences()
		{
			base.InitReferences();

			if ( m_segmentCount > 0 )
			{
				CreateSegments();
			}
		}

		private void CreateSegments()
		{
			SnakeSegment prevSegment = m_headSegment[this];

			m_segments.Capacity = m_segmentCount;
			for ( int idx = 0; idx < m_segmentCount; ++idx )
			{
				SnakeSegment newSegment = CreateSnakeSegment( prevSegment );
				newSegment.name = newSegment.name.Replace( "Clone", idx.ToString() );

				CreateRigSegment( prevSegment, newSegment );

				prevSegment = newSegment;

				m_segments.Add( newSegment );
			}

			PostProcessSegments( m_segments );

			m_rigBuilder[this].Build();
		}

		private void PostProcessSegments( IList<SnakeSegment> snakeSegments )
		{
			for ( int idx = 0; idx < snakeSegments.Count; ++idx )
			{
				SnakeSegment segment = snakeSegments[idx];
				ISliceable sliceable = segment.GetComponent<ISliceable>();

				sliceable.Sliced += OnSegmentSliced;
			}
		}

		private void OnSegmentSliced( object sender, System.EventArgs e )
		{
			Component slicedComponent = sender as Component;
			SnakeSegment slicedSegment = slicedComponent?.GetComponent<SnakeSegment>();
			Debug.Assert( slicedSegment != null, $"Sliced segment must be of type 'SnakeSegment.'", this );

			List<SnakeSegment> splitSegments = null;
			SnakeSegment nextSegment = null;
			for ( int idx = 0; idx < m_segments.Count - 1; ++idx )
			{
				SnakeSegment otherSegment = m_segments[idx];
				if ( otherSegment != slicedSegment ) { continue; }

				int nextSegmentIndex = idx + 1;
				nextSegment = m_segments[nextSegmentIndex];

				splitSegments = m_segments.GetRange( nextSegmentIndex, m_segments.Count - nextSegmentIndex );
				m_segments.RemoveRange( idx, m_segments.Count - idx );

				break;
			}

			if ( nextSegment == null ) { return; }

			foreach ( var segment in splitSegments )
			{
				ISliceable sliceable = segment.GetComponent<ISliceable>();
				sliceable.Sliced -= OnSegmentSliced;
			}


			//Animator rigAnim = m_rigBuilder[this].GetComponent<Animator>();
			////rigAnim.enabled = false;

			////m_rigBuilder[this].Clear();
			////gameObject.SetActive( false );


			//nextSegment.transform.SetParent( null, true );
			//nextSegment.name = nextSegment.name.Replace( "Segment", "NewHead" );


			////m_rigBuilder[this].Clear();
			////gameObject.SetActive( false );

			//Transform rigParent = m_rig[this].transform.parent;
			//GameObject newRigObj = new GameObject( "Rig2" );
			//newRigObj.transform.SetParent( rigParent, false );

			////Destroy( m_rig[this].gameObject );
			//m_rig[this].gameObject.SetActive( false );
			////m_rigBuilder[this].layers.Clear();
			////int lastRigIndex = m_rigBuilder[this].layers.Count - 1;
			////m_rigBuilder[this].layers[lastRigIndex].active = false;
			////m_rigBuilder[this].layers[lastRigIndex].rig.weight = 0;

			//m_rig[this] = newRigObj.AddComponent<Rig>();
			//m_rigBuilder[this].layers = new List<RigLayer>() { new RigLayer( m_rig[this] ) };

			//for ( int sdx = 1; sdx < m_segments.Count; ++sdx )
			//{
			//	var prev = m_segments[sdx - 1];
			//	var next = m_segments[sdx];
			//	CreateRigSegment( prev, next );
			//}


			////gameObject.SetActive( true );
			////m_rigBuilder[this].Build();

			////m_rigBuilder[this].Clear();

			//////GameObject animObj = m_rigBuilder[this].gameObject;
			////Animator rigAnimator = m_rigBuilder[this].GetComponent<Animator>();
			//////rigAnimator.enabled = false;
			//////m_rigBuilder[this].enabled = false;
			//////Destroy( m_rigBuilder[this] );
			//////Destroy( rigAnimator );
			////gameObject.SetActive( false );

			////for ( int cdx = m_rig[this].transform.childCount - 1; cdx >= m_segments.Count; --cdx )
			////{
			////	Transform childDampedSegment = m_rig[this].transform.GetChild( cdx );
			////	Destroy( childDampedSegment.gameObject );
			////}

			//////animObj.AddComponent<Animator>();
			//////m_rigBuilder[this] = animObj.AddComponent<RigBuilder>();
			////m_rigBuilder[this].layers = new List<RigLayer>() { new RigLayer( m_rig[this] ) };


			////m_rigBuilder[this].Build();

			////gameObject.SetActive( true );


			////rigAnim.enabled = true;
			////rigAnim.Rebind();
			////m_rigBuilder[this].enabled = true;

			////m_rigBuilder[this].Build();
			//////anim.enabled = true;
			////anim.Rebind();



			////nextSegment.transform.SetParent( null, true );
			////nextSegment.name = nextSegment.name.Replace( "Segment", "NewHead" );

			//m_rigBuilder[this].Build();
			//rigAnim.enabled = true;
			//rigAnim.Rebind();
			//gameObject.SetActive( true );

			nextSegment.transform.SetParent( null, true );
			nextSegment.name = nextSegment.name.Replace( "Segment", "NewHead" );

			SnakeHead newHead = nextSegment.gameObject.AddComponent<SnakeHead>();
			newHead.m_segmentCount = 0;
			newHead.m_segments = splitSegments;

			for ( int idx = 1; idx < splitSegments.Count; ++idx )
			{
				var prev = splitSegments[idx - 1];
				var next = splitSegments[idx];
				newHead.CreateRigSegment( prev, next );
			}

			newHead.PostProcessSegments( splitSegments );

			newHead.m_rigBuilder[newHead].Build();
		}

		private SnakeSegment CreateSnakeSegment( SnakeSegment prevSegment )
		{
			Quaternion parentRot = prevSegment.Segment.rotation;
			Vector3 parentPos = prevSegment.Segment.position;
			Vector3 offset = parentRot * Vector3.down * prevSegment.Radius * 2;
			Vector3 spawnPos = parentPos + offset;

			SnakeSegment newSegment = Instantiate( m_segmentPrefab, spawnPos, parentRot, prevSegment.Segment );
			newSegment.gameObject.SetActive( true );

			return newSegment;
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
	}
}
