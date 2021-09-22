using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Player
{
	[RequireComponent( typeof( LineRenderer ) )]
    public class PlayerTrajectoryRenderer : MonoBehaviour
	{
		private const int k_minLinePoints = 2;

		[SerializeField] private Transform m_reticle = default;

		private LineRenderer m_renderer;
		private PlayerTrajectoryController m_dragAndDrop;

		private void OnZipUpCompleted( object sender, DragArgs e )
		{
			m_renderer.positionCount = 0;
			m_reticle.gameObject.SetActive( false );
		}

		private void OnDragStarted( object sender, DragArgs e )
		{
			SetPositions( e );
			UpdateReticle( e.End );
		}

		private void OnDragUpdated( object sender, DragArgs e )
		{
			SetPositions( e );
			UpdateReticle( e.End );
		}

		private void SetPositions( DragArgs args )
		{
			if ( m_renderer.positionCount < k_minLinePoints )
			{
				m_renderer.positionCount = k_minLinePoints;
			}

			m_renderer.SetPosition( 0, args.Start );
			m_renderer.SetPosition( 1, args.End );
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
			m_dragAndDrop.ZipUpCompleted += OnZipUpCompleted;
		}

		private void OnDestroy()
		{
			m_dragAndDrop.DragStarted -= OnDragStarted;
			m_dragAndDrop.DragUpdated -= OnDragUpdated;
			m_dragAndDrop.ZipUpCompleted -= OnZipUpCompleted;
		}

		private void Awake()
		{
			m_renderer = GetComponent<LineRenderer>();
			m_dragAndDrop = GetComponentInParent<PlayerTrajectoryController>();
		}
	}
}