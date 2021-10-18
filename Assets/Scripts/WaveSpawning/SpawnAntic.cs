using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.WaveSpawning
{
    public class SpawnAntic : MonoBehaviour
    {
        public float Duration => m_duration;

        [SerializeField, Min( 0 )] private float m_duration = 1;
    }
}
