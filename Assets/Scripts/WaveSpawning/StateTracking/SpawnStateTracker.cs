using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.WaveSpawning
{
    using EventQueues;

    public class SpawnStateTracker<TSpawnable> : MonoBehaviour
        where TSpawnable : Component
    {
        public virtual PlayState State { get; protected set; }

        protected int m_expectedSpawnCount;

        public virtual void PrePlay( int expectedSpawnCount )
        {
            State = PlayState.Playing;
            m_expectedSpawnCount = expectedSpawnCount;
        }

        public virtual void Spawned( TSpawnable enemy )
        {

        }

        public virtual void PostPlay()
        {
            State = PlayState.Done;
        }
    }
}
