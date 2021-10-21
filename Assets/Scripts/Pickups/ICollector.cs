using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay
{
    public interface ICollector
    {
        void Collect( Pickup pickup );
    }
}
