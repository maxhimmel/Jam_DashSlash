using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
    public interface IMatryoshkaEnemy
    {
        public Enemy[] GetChildren();
    }
}
