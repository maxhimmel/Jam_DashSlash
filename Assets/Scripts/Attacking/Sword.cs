using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using Xam.Gameplay.Vfx;

namespace DashSlash.Gameplay.Weapons
{
	using Player;

    public class Sword : MonoBehaviour
    {
		private Vector3 SliceTrajectory => transform.up;

		[SerializeField] private float m_sliceForce = 2;

        private Rigidbody2DBucket m_volume;
		private Collider2D m_collider;
		private PlayerTrajectoryController m_trajectoryController;

		private void OnDashStarted( object sender, DragArgs e )
		{
			transform.rotation = Quaternion.LookRotation( Vector3.forward, e.Vector );
			m_collider.enabled = true;
		}

		private void OnDashCompleted( object sender, DragArgs e )
		{
			m_collider.enabled = false;
			m_volume.ClearBucket();
		}

		private void OnTargetEnteredVolume( object sender, Rigidbody2D e )
		{
			e.isKinematic = true;

			MeshFilter meshFilter = e.GetComponentInChildren<MeshFilter>();
			if ( meshFilter == null ) { return; }

			e.gameObject.SetActive( false );

			GameObject sliceObj = meshFilter.gameObject;
			Quaternion sliceRot = sliceObj.transform.rotation;
			Vector3 slicePos = sliceObj.transform.position;
			Vector3 sliceNormal = Quaternion.AngleAxis( 90, Vector3.forward ) * SliceTrajectory;

			Debug.DrawRay( slicePos, SliceTrajectory, Color.cyan, 5 );
			Debug.DrawRay( slicePos, sliceNormal, Color.magenta, 5 );

			GameObject[] slices = sliceObj.SliceInstantiate( slicePos, sliceNormal );
			if ( slices == null ) { return; }

			for ( int idx = 0; idx < slices.Length; ++idx )
			{
				GameObject obj = slices[idx];

				MeshFadeEmancipation meshFader = obj.AddComponent<MeshFadeEmancipation>();
				meshFader.Emancipate();

				obj.transform.SetPositionAndRotation( slicePos, sliceRot );
				Rigidbody2D body = obj.AddComponent<Rigidbody2D>();
				body.mass = e.mass * 0.5f;
				body.drag = e.drag * 0.5f;
				body.angularDrag = e.angularDrag * 0.5f;

				int sliceDir = (idx & 1) > 0 ? -1 : 1;
				Vector3 sliceForce = sliceDir * sliceNormal * m_sliceForce;
				body.AddForceAtPosition( sliceForce, slicePos, ForceMode2D.Impulse );
			}

			Destroy( sliceObj );
		}

		private void Start()
		{
			m_collider.enabled = false;

			m_trajectoryController.DragReleased += OnDashStarted;
			m_trajectoryController.ZipUpCompleted += OnDashCompleted;

			m_volume.TargetEntered += OnTargetEnteredVolume;
		}

		private void Awake()
		{
			m_volume = GetComponentInChildren<Rigidbody2DBucket>();
			m_collider = GetComponentInChildren<Collider2D>();
			m_trajectoryController = GetComponentInParent<PlayerTrajectoryController>();
		}
	}
}
