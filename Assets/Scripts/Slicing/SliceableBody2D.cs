using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Slicing
{
	[RequireComponent( typeof( Rigidbody2D ) )]
    public class SliceableBody2D : Sliceable
    {
		private Rigidbody2D m_body;

		protected override void OnPreSlice()
		{
			base.OnPreSlice();

			m_body.isKinematic = true;
		}

		protected override void OnSlicedObjectCreated( GameObject slice )
		{
			base.OnSlicedObjectCreated( slice );

			Rigidbody2D sliceBody = slice.AddComponent<Rigidbody2D>();
			sliceBody.mass = m_body.mass * 0.5f;
			sliceBody.drag = m_body.drag * 0.5f;
			sliceBody.angularDrag = m_body.angularDrag * 0.5f;
		}

		protected override void Awake()
		{
			base.Awake();

			m_body = GetComponent<Rigidbody2D>();
		}
	}
}
