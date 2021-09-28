using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using Xam.Gameplay.Vfx;

namespace DashSlash.Gameplay.Slicing
{
	public class Sliceable : MonoBehaviour, ISliceable
	{
		public event EventHandler Sliced;

		public virtual Vector3 MeshPos => MeshObj.transform.position;

		protected GameObject MeshObj => m_sliceMesh.gameObject;

		[SerializeField, Min( -1 )] private float m_sliceLifetime = 0.65f;
		[SerializeField] private MeshFilter m_sliceMesh = default;

		public GameObject[] Slice( Vector3 position, Vector3 normal )
		{
			GameObject[] slices = MeshObj.SliceInstantiate( position, normal );
			if ( slices.Length <= 0 ) { return slices; }

			OnPreSlice();

			for ( int idx = 0; idx < slices.Length; ++idx )
			{
				GameObject obj = slices[idx];
				obj.transform.SetPositionAndRotation( position, MeshObj.transform.rotation );

				OnSlicedObjectCreated( obj );
			}

			OnSliced();

			return slices;
		}

		protected virtual void OnPreSlice()
		{
			gameObject.SetActive( false );
		}

		protected virtual void OnSliced()
		{
			Destroy( gameObject );

			Sliced?.Invoke( this, EventArgs.Empty );
		}

		protected virtual void OnSlicedObjectCreated( GameObject slice )
		{
			if ( m_sliceLifetime >= 0 )
			{
				MeshFadeEmancipation fadeEmancipation = slice.AddComponent<MeshFadeEmancipation>();
				fadeEmancipation.SetDuration( m_sliceLifetime );
				fadeEmancipation.Emancipate();
			}
		}

		protected virtual void Awake()
		{
			if ( m_sliceMesh == null )
			{
				m_sliceMesh = GetComponentInChildren<MeshFilter>();
			}
		}
	}
}
