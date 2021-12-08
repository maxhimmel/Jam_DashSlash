using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Player
{
	[CreateAssetMenu( fileName = "NewTrajectoryDatum", menuName = "DashSlash/Trajectory/Swipe Controller Datum" )]
	public class SwipeControllerDatum : TrajectoryControllerDatum
	{
		protected override TrajectoryController InstallController( GameObject container )
		{
			return container.AddComponent<PlayerSwipeController>();
		}
	}
}
