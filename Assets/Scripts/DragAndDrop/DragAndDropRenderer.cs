using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay
{
	[RequireComponent( typeof( LineRenderer ) )]
    public class DragAndDropRenderer : MonoBehaviour
	{
		[SerializeField] private Transform m_reticle = default;

		private LineRenderer m_renderer;
		private IDragAndDrop m_dragAndDrop;

		private void OnDragStarted( object sender, DragArgs e )
		{
			m_renderer.positionCount = 2;

			m_renderer.SetPosition( 0, e.Start );
			m_renderer.SetPosition( 1, e.End );

			UpdateReticle( e.End );
		}

		private void OnDragUpdated( object sender, DragArgs e )
		{
			m_renderer.SetPosition( 1, e.End );

			UpdateReticle( e.End );
		}

		private void OnDragReleased( object sender, DragArgs e )
		{
			m_renderer.positionCount = 0;
			m_reticle.gameObject.SetActive( false );
		}

		private void UpdateReticle( Vector3 position )
		{
			m_reticle.gameObject.SetActive( true );
			m_reticle.position = position;
		}

		private void Start()
		{
			m_renderer.positionCount = 0;
			m_reticle.gameObject.SetActive( false );

			m_dragAndDrop.DragStarted += OnDragStarted;
			m_dragAndDrop.DragUpdated += OnDragUpdated;
			m_dragAndDrop.DragReleased += OnDragReleased;
		}

		private void OnDestroy()
		{
			m_dragAndDrop.DragStarted -= OnDragStarted;
			m_dragAndDrop.DragUpdated -= OnDragUpdated;
			m_dragAndDrop.DragReleased -= OnDragReleased;
		}

		private void Awake()
		{
			m_renderer = GetComponent<LineRenderer>();
			m_dragAndDrop = GetComponentInParent<IDragAndDrop>();
		}
	}
}
