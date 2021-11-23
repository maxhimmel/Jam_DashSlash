using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Slicing
{
	public class SliceDirectionBlocker : MonoBehaviour, ISliceResolver
	{
		private Vector3 BlockDirection => transform.up;

		bool ISliceResolver.CanSlice( Vector3 position, Vector3 sliceDirection )
		{
			float dot = Vector3.Dot( sliceDirection, BlockDirection );
			return dot > 0;
		}
	}
}
