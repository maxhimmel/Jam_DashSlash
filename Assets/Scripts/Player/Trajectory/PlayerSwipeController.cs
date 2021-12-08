using UnityEngine;

namespace DashSlash.Gameplay.Player
{
	public class PlayerSwipeController : TrajectoryController
	{
		protected override DragArgs ProcessDragStart( object sender, DragArgs args )
		{
			m_currentDrag = new DragArgs( args );
			m_currentDrag.Start = GetClampedStartPosition( args.Start );
			m_currentDrag.End = m_currentDrag.Start;

			return m_currentDrag;
		}

		private Vector3 GetClampedStartPosition( Vector3 startPos )
		{
			Vector3 direction = Vector3.ClampMagnitude( startPos - Center, m_startProximity );
			return direction + Center;
		}

		protected override DragArgs ProcessDragUpdate( object sender, DragArgs args )
		{
			m_currentDrag.End = GetClampedEndPosition( args );
			return m_currentDrag;
		}

		protected override DragArgs ProcessDragRelease( object sender, DragArgs args )
		{
			m_currentDrag.End = GetClampedEndPosition( args );
			return m_currentDrag;
		}

		private Vector3 GetClampedEndPosition( DragArgs args )
		{
			Vector3 direction = Vector3.ClampMagnitude( args.End - args.Start, m_endProximity );
			return direction + Center;
		}

		protected override Vector3 GetForceUpdatedEndPosition()
		{
			return m_currentDrag.End;
		}
	}
}
