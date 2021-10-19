using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Utility
{
    public interface IDirection
    {
        Vector3 GetDirection( int index, int count );
    }
}
