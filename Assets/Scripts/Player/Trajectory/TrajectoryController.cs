using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player
{
	public abstract class TrajectoryController : MonoBehaviour, ITrajectoryController
	{
		public event EventHandler<DragArgs> DragStarted;
		public event EventHandler<DragArgs> DragUpdated;
		public event EventHandler<DragArgs> DragReleased;
		public event EventHandler<DragArgs> ZipUpCompleted;
		public event EventHandler<DragArgs> TrajectoryConnected;

		public bool IsDragging => enabled && m_dragAndDrop.IsDragging;

		protected Vector3 Center => transform.position;
		private bool IsDragStarted => m_currentDrag != null;

		[Header( "Proximity" )]
		[SerializeField] protected float m_startProximity = 2;
		[SerializeField] protected float m_endProximity = 8;

		private Coroutine m_zipUpRoutine;
		private Tweener m_retrieveReticleTween;

		protected DragAndDrop m_dragAndDrop;
		protected DragArgs m_currentDrag;

		private void OnDragStarted( object sender, DragArgs args )
		{
			this.TryStopCoroutine( ref m_zipUpRoutine );

			var processedArgs = ProcessDragStart( sender, args );

			DragStarted?.Invoke( this, processedArgs );
		}

		protected abstract DragArgs ProcessDragStart( object sender, DragArgs args );

		private void OnDragUpdated( object sender, DragArgs args )
		{
			if ( !IsDragStarted ) { return; }

			var processedArgs = ProcessDragUpdate( sender, args );

			DragUpdated?.Invoke( this, processedArgs );
		}

		protected abstract DragArgs ProcessDragUpdate( object sender, DragArgs args );

		private void OnDragReleased( object sender, DragArgs args )
		{
			if ( !IsDragStarted ) { return; }

			var processedArgs = ProcessDragRelease( sender, args );
			var prevDragArgs = new DragArgs( processedArgs );

			this.TryStopCoroutine( ref m_zipUpRoutine );
			m_zipUpRoutine = StartCoroutine( WaitForZipUpComplete( processedArgs ) );

			DragReleased?.Invoke( this, prevDragArgs );
		}

		protected abstract DragArgs ProcessDragRelease( object sender, DragArgs args );

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

			m_currentDrag = null;
			m_zipUpRoutine = null;

			ZipUpCompleted?.Invoke( this, args );
			TrajectoryConnected?.Invoke( this, args );
		}

		public void ForceUpdate()
		{
			var args = new DragArgs( Center, GetForceUpdatedEndPosition() );
			OnDragUpdated( this, args );
		}

		protected abstract Vector3 GetForceUpdatedEndPosition();

		public void SetActive( bool isActive )
		{
			enabled = isActive;
		}

		public void RetrieveReticle( float duration, Ease easeAnim )
		{
			if ( !IsDragStarted ) { return; }

			this.TryStopCoroutine( ref m_zipUpRoutine );
			if ( m_retrieveReticleTween.IsActive() )
			{
				m_retrieveReticleTween.Kill();
			}

			float timer = 0;
			Vector3 reticleStartPos = m_currentDrag.End;
			var args = new DragArgs( Center, reticleStartPos );

			m_retrieveReticleTween = DOTween.To( () => timer, tweenTime => timer = tweenTime, 1, duration )
				.SetEase( easeAnim )
				.OnUpdate( () =>
				{
					Vector3 newReticlePos = Vector3.LerpUnclamped( reticleStartPos, Center, timer );

					args.Start = Center;
					args.End = newReticlePos;
					DragUpdated?.Invoke( this, args );
				} )
				.OnComplete( () =>
				{
					args.Start = Center;
					args.End = Center;
					TrajectoryConnected?.Invoke( this, args );
				} );

			m_currentDrag = null;
		}

		protected virtual void OnEnable()
		{
			m_dragAndDrop.DragStarted += OnDragStarted;
			m_dragAndDrop.DragUpdated += OnDragUpdated;
			m_dragAndDrop.DragReleased += OnDragReleased;
		}

		protected virtual void OnDisable()
		{
			m_dragAndDrop.DragStarted -= OnDragStarted;
			m_dragAndDrop.DragUpdated -= OnDragUpdated;
			m_dragAndDrop.DragReleased -= OnDragReleased;
		}

		protected virtual void Awake()
		{
			m_dragAndDrop = GetComponentInChildren<DragAndDrop>();
		}


#if UNITY_EDITOR
		[Header( "Editor / Tools" )]
		[SerializeField] private Color m_startProximityColor = Color.red;
		[SerializeField] private Color m_endProximityColor = Color.cyan;

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
