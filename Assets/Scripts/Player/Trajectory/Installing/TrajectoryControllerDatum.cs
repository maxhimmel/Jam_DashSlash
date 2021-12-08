using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Player
{
    public abstract class TrajectoryControllerDatum : ScriptableObject
    {
        [SerializeField] private float m_startProximity = 2;
        [SerializeField] private float m_endProximity = 8;

        public TrajectoryController AddController( GameObject container )
		{
            var controller = InstallController( container );
            controller.SetProximity( m_startProximity, m_endProximity );

            return controller;
		}

        protected abstract TrajectoryController InstallController( GameObject container );
    }
}
