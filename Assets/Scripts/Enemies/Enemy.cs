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

		public virtual Vector3 Position => m_pawn.position;
		public virtual Vector3 FacingDirection => m_pawn.up;

		protected bool IsAwake => m_sleepRoutine == null;

		[Header( "Spawning" )]
        [SerializeField, Min( 0 )] private float m_spawnAwakeDelay = 0.25f;
        [SerializeField, Min( 0 )] private float m_spawnInvincibiltyDuration = 0.25f;

		[Header( "Helpers" )]
		[SerializeField] private LookAtPlayer m_lookAtPlayer = new LookAtPlayer();

		private Transform m_pawn;
		private Coroutine m_sleepRoutine;
		private Coroutine m_spawnInvincibilityRoutine;
		private ISliceable m_sliceable;
		private HurtBox[] m_hurtBoxes;

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
            transform.rotation = m_lookAtPlayer.GetRotation( Position );
		}

		protected Vector3 GetDirectionToPlayer()
		{
			return m_lookAtPlayer.GetDirection( Position );
		}

		protected float GetDistanceToPlayer()
		{
			return m_lookAtPlayer.GetDistance( Position );
		}

		protected float GetDistanceSqrToPlayer()
		{
			return m_lookAtPlayer.GetDistanceSqr( Position );
		}

		protected virtual void OnSliced( object sender, System.EventArgs e )
		{
			Died?.Invoke( this, e );
		}

		private void OnEnable()
		{
			this.TryStopCoroutine( ref m_sleepRoutine );
			m_sleepRoutine = this.StartWaitingForSeconds( m_spawnAwakeDelay, OnAwokenFromSpawn );

			if ( m_spawnInvincibiltyDuration > 0 )
			{
				BeginSpawnInvincibilityToggle();
			}
		}

		protected virtual void OnAwokenFromSpawn()
		{
			m_sleepRoutine = null;
		}

		private void BeginSpawnInvincibilityToggle()
		{
			SetHurtBoxesActive( false );

			this.TryStopCoroutine( ref m_spawnInvincibilityRoutine );
			m_spawnInvincibilityRoutine = this.StartWaitingForSeconds( m_spawnInvincibiltyDuration, () =>
			{
				SetHurtBoxesActive( true );
			} );
		}

		private void SetHurtBoxesActive( bool isActive )
		{
			foreach ( HurtBox hurt in m_hurtBoxes )
			{
				hurt.enabled = isActive;
			}
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
			m_pawn = transform.Find( "Pawn" );

			m_body = GetComponent<Rigidbody2D>();
			m_sliceable = GetComponentInChildren<ISliceable>();
			m_hurtBoxes = GetComponentsInChildren<HurtBox>( true );
		}

		protected virtual void OnDisable()
		{

		}
	}
}
