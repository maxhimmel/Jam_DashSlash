using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player
{
    public class PlayerTrajectoryController : MonoBehaviour, IDragAndDrop
    {
		public event EventHandler<DragArgs> DragStarted;
		public event EventHandler<DragArgs> DragUpdated;
		public event EventHandler<DragArgs> DragReleased;
		public event EventHandler<DragArgs> ZipUpCompleted;

		public bool IsDragging => enabled && m_dragAndDrop.IsDragging;
		public Vector3 Trajectory => m_dragAndDrop.CurrentDrag.Vector;

		private Vector3 Center => transform.position;

		[Header( "Proximity" )]
		[SerializeField] private float m_startProximity = 2;
		[SerializeField] private float m_endProximity = 8;

		private DragAndDrop m_dragAndDrop;
		private Coroutine m_zipUpRoutine;

		private void OnDragStarted( object sender, DragArgs args )
		{
			this.TryStopCoroutine( ref m_zipUpRoutine );

			args.Start = GetClampedPosition( args.Start, m_startProximity );
			args.End = GetClampedPosition( args.End, m_endProximity );

			DragStarted?.Invoke( this, args );
		}

		private void OnDragUpdated( object sender, DragArgs args )
		{
			args.End = GetClampedPosition( args.End, m_endProximity );

			DragUpdated?.Invoke( this, args );
		}

		private void OnDragReleased( object sender, DragArgs args )
		{
			args.End = GetClampedPosition( args.End, m_endProximity );

			this.TryStopCoroutine( ref m_zipUpRoutine );
			m_zipUpRoutine = StartCoroutine( WaitForZipUpComplete( args ) );
			
			DragReleased?.Invoke( this, args );
		}

		private Vector3 GetClampedPosition( Vector3 position, float proximity )
		{
			Vector3 direction = Vector3.ClampMagnitude( position - Center, proximity );
			return direction + Center;
		}

		private IEnumerator WaitForZipUpComplete( DragArgs args )
		{
			float remainingDistSqr = (Center - args.End).sqrMagnitude;
			while ( remainingDistSqr > 0.01f )
			{
				args.Start = Center;
				OnDragUpdated( this, args );

				remainingDistSqr = (Center - args.End).sqrMagnitude;
				yield return null;
			}

			m_zipUpRoutine = null;
			ZipUpCompleted?.Invoke( this, args );
		}

		public void ForceUpdate()
		{
			var args = new DragArgs( Center, m_dragAndDrop.GetMouseWorldPosition() );
			OnDragUpdated( this, args );
		}

		private void OnEnable()
		{
			m_dragAndDrop.DragStarted += OnDragStarted;
			m_dragAndDrop.DragUpdated += OnDragUpdated;
			m_dragAndDrop.DragReleased += OnDragReleased;
		}

		private void OnDisable()
		{
			m_dragAndDrop.DragStarted -= OnDragStarted;
			m_dragAndDrop.DragUpdated -= OnDragUpdated;
			m_dragAndDrop.DragReleased -= OnDragReleased;
		}

		private void Awake()
		{
			m_dragAndDrop = GetComponentInChildren<DragAndDrop>();
		}


#if UNITY_EDITOR
		[Header( "Editor / Tools" )]
		[SerializeField] private Color m_startProximityColor = Color.red;
		[SerializeField] private Color m_endProximityColor = Color.green;

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = m_startProximityColor;
			Gizmos.DrawWireSphere( Center, m_startProximity );

			Gizmos.color = m_endProximityColor;
			Gizmos.DrawWireSphere( Center, m_endProximity );
		}
#endif
	}
}
