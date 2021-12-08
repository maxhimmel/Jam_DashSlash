using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Player
{
	[CreateAssetMenu( fileName = "NewTrajectoryDatum", menuName = "DashSlash/Trajectory/Point + Click Controller Datum" )]
	public class PointControllerDatum : TrajectoryControllerDatum
	{
		protected override TrajectoryController InstallController( GameObject container )
		{
			return container.AddComponent<PlayerTrajectoryController>();
		}
	}
}
