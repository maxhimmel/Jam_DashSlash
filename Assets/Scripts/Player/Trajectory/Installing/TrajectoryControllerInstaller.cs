using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player
{
    public class TrajectoryControllerInstaller : MonoBehaviour
    {
        private ITrajectoryControllerContainer[] m_containers;

		public void Install( ITrajectoryController controller )
		{
			foreach ( var container in m_containers )
			{
				container.InstallTrajectory( controller );
			}
		}

		private void Awake()
		{
			m_containers = GetComponentsInChildren<ITrajectoryControllerContainer>( true );
		}
	}
}
