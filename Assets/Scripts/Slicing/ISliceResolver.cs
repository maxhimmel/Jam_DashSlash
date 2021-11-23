using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Slicing
{
	public class SimpleSliceResolver : ISliceResolver
	{
		bool ISliceResolver.CanSlice( Vector3 position, Vector3 sliceDirection )
		{
			return true;
		}
	}

	public interface ISliceResolver
    {
        bool CanSlice( Vector3 position, Vector3 sliceDirection );
    }
}
