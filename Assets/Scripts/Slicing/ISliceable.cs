using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Slicing
{
    public interface ISliceable
    {
        event System.EventHandler Sliced;

        public Vector3 MeshPos { get; }

        GameObject[] Slice( Vector3 position, Vector3 normal );
    }
}
