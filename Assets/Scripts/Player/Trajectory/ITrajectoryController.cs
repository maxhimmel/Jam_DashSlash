using System;
using DG.Tweening;

namespace DashSlash.Gameplay.Player
{
    public interface ITrajectoryController : IDragAndDrop
	{
		public event EventHandler<DragArgs> ZipUpCompleted;
		public event EventHandler<DragArgs> TrajectoryConnected;

		public bool IsDragging { get; }

		void SetActive( bool isActive );
		void ForceUpdate();
		void RetrieveReticle( float duration, Ease easeAnim );
	}
}
