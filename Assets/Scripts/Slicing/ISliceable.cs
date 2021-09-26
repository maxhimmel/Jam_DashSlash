using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Slicing
{
    public interface ISliceable
    {
        event System.EventHandler Sliced;

        GameObject[] Slice( Vector3 position, Vector3 normal );
    }
}
