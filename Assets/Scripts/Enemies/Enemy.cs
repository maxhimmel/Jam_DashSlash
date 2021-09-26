using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Enemies
{
    using Player;

    public class Enemy : MonoBehaviour
    {
		protected bool IsAwake => m_sleepRoutine == null;

        [SerializeField, Min( 0 )] private float m_spawnAwakeDelay = 0.25f;
        [SerializeField] protected LookAtPlayer m_lookAtPlayer = new LookAtPlayer();

		private Coroutine m_sleepRoutine;
		protected Rigidbody2D m_body;

        private void FixedUpdate()
		{
			if ( IsAwake )
			{
				UpdateState();
			}
		}

		protected virtual void UpdateState()
		{
			UpdateRotationTowardsPlayer();
		}

        protected void UpdateRotationTowardsPlayer()
		{
            transform.rotation = m_lookAtPlayer.GetRotation( transform.position );
		}

		private void OnEnable()
		{
			this.TryStopCoroutine( ref m_sleepRoutine );
			m_sleepRoutine = this.StartWaitingForSeconds( m_spawnAwakeDelay, OnAwokenFromSpawn );
		}

		protected virtual void OnAwokenFromSpawn()
		{
			m_sleepRoutine = null;
		}

		protected virtual void Awake()
		{
			m_body = GetComponent<Rigidbody2D>();
		}

		protected virtual void OnDisable()
		{

		}
	}
}
