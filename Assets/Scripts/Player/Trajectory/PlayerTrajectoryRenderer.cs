using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player
{
	[RequireComponent( typeof( LineRenderer ) )]
    public class PlayerTrajectoryRenderer : MonoBehaviour,
		ITrajectoryControllerContainer
	{
		private const int k_minLinePoints = 2;

		[SerializeField] private Transform m_reticle = default;

		private LineRenderer m_renderer;
		private ITrajectoryController m_trajectoryController;

		void ITrajectoryControllerContainer.InstallTrajectory( ITrajectoryController controller )
		{
			if ( m_trajectoryController != null )
			{
				m_trajectoryController.ZipUpCompleted -= OnTrajectoryCompleted;
				m_trajectoryController.DragStarted -= OnDragStarted;
				m_trajectoryController.DragUpdated -= OnDragUpdated;
			}

			if ( controller != null )
			{
				controller.TrajectoryConnected += OnTrajectoryCompleted;
				controller.DragStarted += OnDragStarted;
				controller.DragUpdated += OnDragUpdated;
			}

			m_trajectoryController = controller;
		}

		private void OnTrajectoryCompleted( object sender, DragArgs e )
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

			if ( !m_renderer.useWorldSpace )
			{
				args = args.ToLocalSpace( transform );
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

			ITrajectoryControllerContainer trajectoryContainer = this;
			trajectoryContainer.InstallTrajectory( m_trajectoryController );
		}

		private void OnDestroy()
		{
			m_trajectoryController.ZipUpCompleted -= OnTrajectoryCompleted;
			m_trajectoryController.DragStarted -= OnDragStarted;
			m_trajectoryController.DragUpdated -= OnDragUpdated;
		}

		private void Awake()
		{
			m_renderer = GetComponent<LineRenderer>();
			m_trajectoryController = GetComponentInParent<ITrajectoryController>();
		}
	}
}
