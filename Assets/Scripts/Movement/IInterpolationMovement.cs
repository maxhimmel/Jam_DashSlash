using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Movement
{
    public interface IInterpolationMovement
	{
		bool IsMoving { get; }
		Vector3 Position { get; }
		Vector3 Velocity { get; }

		void SetDuration( float duration );
		void SetEase( Ease ease );
		void SetDesiredVelocity( Vector3 direction );
		void ClearMovement();
	}
}
