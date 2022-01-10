using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Enemies
{
	using Slicing;
    using Player;
	using Weapons;
	using Vfx.Googly;
	using Scoring;
	using Xam.Utility.Patterns;

	public class Enemy : MonoBehaviour
    {
		public event System.EventHandler Died;

		public virtual Vector3 Position => m_pawn.position;
		public virtual Vector3 FacingDirection => m_pawn.up;

		protected bool IsAwake => m_sleepRoutine == null;
		protected ScoreController Score => ScoreController.Instance;

		[Header( "Spawning" )]
        [SerializeField, Min( 0 )] protected float m_spawnAwakeDelay = 0.25f;
        [SerializeField, Min( 0 )] protected float m_spawnInvincibiltyDuration = 0.25f;

		[Header( "Rotation" )]
		[SerializeField] protected float m_rotationSpeed = 540;

		[Header( "Scoring" )]
		[SerializeField] private int m_points = 100;

		private bool m_isAppQuitting = false;
		private Transform m_pawn;
		private Coroutine m_sleepRoutine;
		private Coroutine m_spawnInvincibilityRoutine;
		private ISliceable m_sliceable;
		private HurtBox[] m_hurtBoxes;
		private HitBox[] m_hitBoxes;
		private LookAtPlayer m_lookAtPlayer = new LookAtPlayer();
		private VfxPointsFactory m_vfxPointsFactory;

		protected Rigidbody2D m_body;
		protected LootSpawner m_lootSpawner;
		protected GooglyEyesController m_googlyEyes;

		private void FixedUpdate()
		{
			if ( IsAwake )
			{
				UpdateState();
			}
		}

		protected virtual void UpdateState()
		{
			UpdateRotation();
			UpdateGooglyEyes();
		}

		protected virtual void UpdateGooglyEyes()
		{
			if ( m_googlyEyes != null )
			{
				Vector3 directionToPlayer = GetDirectionToPlayer();
				m_googlyEyes.SetDesiredLookDirection( directionToPlayer );
			}
		}

        protected void UpdateRotation()
		{
			Quaternion targetRotation = GetDesiredRotation();

			float rotationDelta = m_rotationSpeed * Time.deltaTime;
			Quaternion newRotation = Quaternion.RotateTowards( transform.rotation, targetRotation, rotationDelta );

			transform.rotation = newRotation;
		}

		protected virtual Quaternion GetDesiredRotation()
		{
			return GetFacingRotationToPlayer();
		}

		protected Quaternion GetFacingRotationToPlayer()
		{
			return m_lookAtPlayer.GetRotation( Position );
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

		protected bool IsPlayerAttacking()
		{
			var player = DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();
			return player.IsAttacking;
		}

		protected Vector3 GetPlayerVelocity()
		{
			var player = DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();
			return player.Velocity;
		}

		private void OnEnable()
		{
			BeginSpawning();
		}

		protected virtual void BeginSpawning()
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
			SetBehaviourEnabled( false, m_hurtBoxes );
			SetBehaviourEnabled( false, m_hitBoxes );

			this.TryStopCoroutine( ref m_spawnInvincibilityRoutine );
			m_spawnInvincibilityRoutine = this.StartWaitingForSeconds( m_spawnInvincibiltyDuration, OnSpawnInvincibilityEnded );
		}

		protected virtual void OnSpawnInvincibilityEnded()
		{
			SetBehaviourEnabled( true, m_hurtBoxes );
			SetBehaviourEnabled( true, m_hitBoxes );
		}

		protected void SetBehaviourEnabled<T>( bool isEnabled, params T[] behaviours ) where T : Behaviour
		{
			foreach ( var behaviour in behaviours )
			{
				behaviour.enabled = isEnabled;
			}
		}

		protected virtual void OnSliced( object sender, System.EventArgs e )
		{
			// The child sliceable will destroy this gameobject.
				// So, we can handle death stuffs in OnDestroy ...

			int points = Score.AddSliceKill( m_points );

			var pointsVfx = m_vfxPointsFactory.Create( Position );
			pointsVfx.SetText( points.ToString() );
		}

		private void OnDestroy()
		{
			if ( Application.isPlaying && !m_isAppQuitting )
			{
				OnDied();
			}
		}

		protected virtual void OnDied()
		{
			if ( m_lootSpawner != null )
			{
				m_lootSpawner.Spawn( Position );
			}

			Died?.Invoke( this, System.EventArgs.Empty );
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
			m_hitBoxes = GetComponentsInChildren<HitBox>( true );
			m_googlyEyes = GetComponentInChildren<GooglyEyesController>( true );
			m_vfxPointsFactory = GetComponentInChildren<VfxPointsFactory>();

			InitLootSpawner();
		}

		private void InitLootSpawner()
		{
			var lootSpawnerFactory = GetComponentInChildren<LootSpawnerFactory>();
			if ( lootSpawnerFactory != null )
			{
				m_lootSpawner = lootSpawnerFactory.Create( Vector3.zero, Quaternion.identity, lootSpawnerFactory.transform );
			}
		}

		protected virtual void OnDisable()
		{

		}

		private void OnApplicationQuit()
		{
			m_isAppQuitting = true;
		}

		private void OnDrawGizmos()
		{
			DrawGizmos();
		}
		protected virtual void DrawGizmos() { }

		private void OnDrawGizmosSelected()
		{
			DrawGizmosSelected();
		}
		protected virtual void DrawGizmosSelected() { }
	}
}
