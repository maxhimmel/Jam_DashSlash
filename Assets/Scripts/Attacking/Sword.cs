using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Randomization;

namespace DashSlash.Gameplay.Weapons
{
	using Player;
	using Gameplay.Slicing;
	using Vfx;

	public class Sword : MonoBehaviour
    {
		private Vector3 SliceTrajectory => transform.up;

		[Header( "Physics" )]
		[SerializeField] private RandomFloatRange m_sliceForceRange = new RandomFloatRange( 1, 2 );
		[SerializeField] private RandomFloatRange m_slicePosOffsetRange = new RandomFloatRange( 0.5f, 1f );

		private Rigidbody2DBucket m_volume;
		private Collider2D m_collider;
		private PlayerTrajectoryController m_trajectoryController;
		private SwordSliceVfxController m_vfxController;

		private void OnDragStarted( object sender, DragArgs e )
		{
			m_collider.enabled = false;
		}

		private void OnDashStarted( object sender, DragArgs e )
		{
			transform.rotation = Quaternion.LookRotation( Vector3.forward, e.Vector );
			m_collider.enabled = true;
		}

		private void OnDashCompleted( object sender, DragArgs e )
		{
			m_collider.enabled = false;
			m_volume.ClearBucket();

			m_vfxController.StopSliceVfx();
		}

		private void OnTargetEnteredVolume( object sender, Rigidbody2D e )
		{
			if ( TrySlice( e ) )
			{
				m_vfxController.PlaySliceVfx();
			}
		}

		private bool TrySlice( Rigidbody2D body )
		{
			if ( body == null ) { return false; }

			ISliceable sliceable = body.GetComponent<ISliceable>();
			if ( sliceable == null ) { return false; }

			Vector3 slicePos = sliceable.MeshPos;
			Vector3 sliceNormal = Quaternion.AngleAxis( 90, Vector3.forward ) * SliceTrajectory;
			GameObject[] slices = sliceable.Slice( slicePos, sliceNormal );

			if ( slices.Length <= 0 ) { return false; }

			for ( int idx = 0; idx < slices.Length; ++idx )
			{
				GameObject obj = slices[idx];
				Rigidbody2D halfBody = obj.GetComponent<Rigidbody2D>();
				if ( halfBody == null ) { continue; }

				int sliceDir = (idx & 1) > 0 ? -1 : 1;
				Vector3 sliceForce = sliceDir * sliceNormal * m_sliceForceRange.Evaluate();
				Vector3 forcePos = slicePos + SliceTrajectory * m_slicePosOffsetRange.Evaluate();

				halfBody.AddForceAtPosition( sliceForce, forcePos, ForceMode2D.Impulse );
			}

			return true;
		}

		private void Start()
		{
			m_collider.enabled = false;

			m_trajectoryController.DragStarted += OnDragStarted;
			m_trajectoryController.DragReleased += OnDashStarted;
			m_trajectoryController.ZipUpCompleted += OnDashCompleted;

			m_volume.TargetEntered += OnTargetEnteredVolume;
		}

		private void Awake()
		{
			m_volume = GetComponentInChildren<Rigidbody2DBucket>();
			m_collider = GetComponentInChildren<Collider2D>();
			m_trajectoryController = GetComponentInParent<PlayerTrajectoryController>();
			m_vfxController = GetComponentInChildren<SwordSliceVfxController>();
		}
	}
}
