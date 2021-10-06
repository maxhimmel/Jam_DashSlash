using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay.Enemies
{
    [System.Serializable]
    public class WeightedEnemyList : WeightedList<WeightedEnemyNode, Enemy> 
    { 
    }

    [System.Serializable]
    public class WeightedEnemyNode : WeightedNode<Enemy> 
    {
    }
}
