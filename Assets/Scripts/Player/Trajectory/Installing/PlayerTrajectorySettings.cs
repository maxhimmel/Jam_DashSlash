using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;
using Sirenix.OdinInspector;

namespace DashSlash.Gameplay.Player
{
    public class PlayerTrajectorySettings : MonoBehaviour
    {
        [Header( "Settings" )]
        [SerializeField] private SwipeControllerDatum m_swipeTrajectory = default;
        [SerializeField] private PointControllerDatum m_pointClickTrajectory = default;

        [PropertySpace]
        [HorizontalGroup( "Options", VisibleIf = "IsAppPlaying" )]
        [Button( DirtyOnClick = false )]
        public void SetSwipeControls()
        {
            ConfigureControls( m_swipeTrajectory );
        }

        [PropertySpace]
        [HorizontalGroup( "Options", VisibleIf = "IsAppPlaying" )]
        [Button( DirtyOnClick = false )]
        public void SetPointClickControls()
		{
            ConfigureControls( m_pointClickTrajectory );
		}

        private void ConfigureControls( TrajectoryControllerDatum controlData )
        {
            if ( TryGetPlayer( out var player ) )
            {
                RemoveControls();

                var newControls = controlData.AddController( player.gameObject );
                SetControls( newControls );
            }
        }

        private void RemoveControls()
		{
            if ( TryGetPlayer( out var player ) )
			{
                if ( player.TryGetComponent<ITrajectoryController>( out var controller ) )
				{
                    Destroy( controller as Component );
				}
			}
		}

        private void SetControls( ITrajectoryController newController )
		{
            if ( TryGetPlayer( out var player ) )
            {
                if ( player.TryGetComponent<TrajectoryControllerInstaller>( out var installer ) )
				{
                    installer.Install( newController );
				}
            }
		}

        private bool TryGetPlayer( out PlayerController player )
        {
            player = DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();
            return player != null;
        }

        private bool IsAppPlaying()
		{
            return Application.isPlaying;
		}
    }
}
