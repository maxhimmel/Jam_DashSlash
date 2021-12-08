using UnityEngine;

namespace DashSlash.Gameplay.Player
{
    public class PlayerTrajectoryController : TrajectoryController
    {
		protected override DragArgs ProcessDragStart( object sender, DragArgs args )
		{
			args.Start = GetClampedPosition( args.Start, m_startProximity );
			args.End = GetClampedPosition( args.End, m_endProximity );
			m_currentDrag = args;

			return args;
		}

		protected override DragArgs ProcessDragUpdate( object sender, DragArgs args )
		{
			args.End = GetClampedPosition( args.End, m_endProximity );
			m_currentDrag = args;

			return args;
		}

		protected override DragArgs ProcessDragRelease( object sender, DragArgs args )
		{
			args.End = GetClampedPosition( args.End, m_endProximity );
			return args;
		}

		private Vector3 GetClampedPosition( Vector3 position, float proximity )
		{
			Vector3 direction = Vector3.ClampMagnitude( position - Center, proximity );
			return direction + Center;
		}

		protected override Vector3 GetForceUpdatedEndPosition()
		{
			return m_dragAndDrop.GetMouseWorldPosition();
		}
	}
}
