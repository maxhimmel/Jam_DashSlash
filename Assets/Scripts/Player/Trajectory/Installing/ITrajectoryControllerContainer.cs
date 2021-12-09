using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Player
{
    public interface ITrajectoryControllerContainer
    {
        void InstallTrajectory( ITrajectoryController controller );
    }
}
