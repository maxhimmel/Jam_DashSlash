using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Enemies
{
	using Slicing;
    using Player;

    public class Enemy : MonoBehaviour
    {
		public event System.EventHandler Died;

		protected bool IsAwake => m_sleepRoutine == null;

        [SerializeField, Min( 0 )] private float m_spawnAwakeDelay = 0.25f;
        [SerializeField] private LookAtPlayer m_lookAtPlayer = new LookAtPlayer();

		private Coroutine m_sleepRoutine;
		private ISliceable m_sliceable;

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

		protected Vector3 GetDirectionToPlayer()
		{
			return m_lookAtPlayer.GetDirection( transform.position );
		}

		protected virtual void OnAwokenFromSpawn()
		{
			m_sleepRoutine = null;
		}

		protected virtual void OnSliced( object sender, System.EventArgs e )
		{
			Died?.Invoke( this, e );
		}

		private void OnEnable()
		{
			this.TryStopCoroutine( ref m_sleepRoutine );
			m_sleepRoutine = this.StartWaitingForSeconds( m_spawnAwakeDelay, OnAwokenFromSpawn );
		}

		private void Start()
		{
			InitReferences();
		}

		protected virtual void InitReferences()
		{
			m_sliceable.Sliced += OnSliced;
		}

		private void Awake()
		{
			CacheReferences();
		}

		protected virtual void CacheReferences()
		{
			m_body = GetComponent<Rigidbody2D>();
			m_sliceable = GetComponentInChildren<ISliceable>();
		}

		protected virtual void OnDisable()
		{

		}
	}
}
