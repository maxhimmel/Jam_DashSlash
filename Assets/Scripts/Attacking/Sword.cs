using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Weapons
{
	using Player;
	using Gameplay.Slicing;

	public class Sword : MonoBehaviour
    {
		public event System.EventHandler Sliced;

		public bool IsSlicing => m_collider.enabled;

		private Vector3 SliceTrajectory => transform.up;

		[Header( "Physics" )]
		[SerializeField] private RandomFloatRange m_sliceForceRange = new RandomFloatRange( 1, 2 );
		[SerializeField] private RandomFloatRange m_slicePosOffsetRange = new RandomFloatRange( 0.5f, 1f );

		private Rigidbody2DBucket m_volume;
		private Collider2D m_collider;
		private Rigidbody2D m_body;
		private SwordSliceVfxController m_vfxController;
		private Coroutine m_collisionCheckRoutine;

		public void SetRotation( Vector3 lookDirection )
		{
			m_body.SetRotation( Quaternion.LookRotation( Vector3.forward, lookDirection ) );
			m_body.MoveRotation( Quaternion.LookRotation( Vector3.forward, lookDirection ) );
		}

		public void StopSlicing( bool stopVfx )
		{
			ClearCollision();

			if ( stopVfx )
			{
				m_vfxController.StopSliceVfx();
			}
		}

		private void ClearCollision()
		{
			m_collider.enabled = false;
			m_volume.ClearBucket();
		}

		public void StartSlicing()
		{
			m_collider.enabled = true;

			this.TryStopCoroutine( ref m_collisionCheckRoutine );
			m_collisionCheckRoutine = StartCoroutine( UpdateCollisionCheck() );
		}

		private IEnumerator UpdateCollisionCheck()
		{
			while ( m_collider.enabled )
			{
				m_volume.CheckProximity();
				yield return new WaitForFixedUpdate();
			}

			m_collisionCheckRoutine = null;
		}

		private void OnTargetEnteredVolume( object sender, Rigidbody2D e )
		{
			if ( TrySlice( e ) )
			{
				m_vfxController.PlaySliceVfx();
				m_vfxController.IncrementSlomoDuration();
			}
		}

		private bool TrySlice( Rigidbody2D body )
		{
			if ( body == null ) { return false; }

			ISliceable sliceable = body.GetComponent<ISliceable>();
			if ( sliceable == null ) { return false; }

			Vector3 slicePos = sliceable.MeshPos;
			Vector3 sliceTrajectory = (slicePos - transform.position).normalized;
			Vector3 sliceNormal = Quaternion.AngleAxis( 90, Vector3.forward ) * sliceTrajectory;
			GameObject[] slices = sliceable.Slice( slicePos, sliceNormal );

			if ( slices.Length <= 0 ) { return false; }

			for ( int idx = 0; idx < slices.Length; ++idx )
			{
				GameObject obj = slices[idx];
				Rigidbody2D halfBody = obj.GetComponent<Rigidbody2D>();
				if ( halfBody == null ) { continue; }

				int sliceDir = (idx & 1) > 0 ? -1 : 1;
				Vector3 sliceForce = sliceDir * sliceNormal * m_sliceForceRange.Evaluate();
				Vector3 forcePos = slicePos + sliceTrajectory * m_slicePosOffsetRange.Evaluate();

				halfBody.AddForceAtPosition( sliceForce, forcePos, ForceMode2D.Impulse );
			}

			Sliced?.Invoke( this, System.EventArgs.Empty );
			return true;
		}

		private void Start()
		{
			ClearCollision();

			m_volume.TargetEntered += OnTargetEnteredVolume;
		}

		private void Awake()
		{
			m_volume = GetComponentInChildren<Rigidbody2DBucket>();
			m_collider = GetComponentInChildren<Collider2D>();
			m_body = GetComponentInChildren<Rigidbody2D>();
			m_vfxController = GetComponentInChildren<SwordSliceVfxController>();
		}
	}
}
