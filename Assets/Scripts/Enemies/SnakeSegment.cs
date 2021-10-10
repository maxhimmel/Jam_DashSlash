using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
    public class SnakeSegment : MonoBehaviour
    {
        public Transform Segment => m_segment;
        public float Radius => m_radius;
        public float DampPosition => m_dampPosition;
        public float DampRotation => m_dampRotation;
        public bool MaintainAim => m_maintainAim;

        [SerializeField] private Transform m_segment = default;

        [Space]
        [SerializeField] private float m_radius = 0.5f;

        [Space]
        [SerializeField, Range( 0, 1 )] private float m_dampPosition = 0;
        [SerializeField, Range( 0, 1 )] private float m_dampRotation = 0.6f;
        [SerializeField] private bool m_maintainAim = false;
    }
}
