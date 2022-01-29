using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace DashSlash
{
    public class ArenaBounds : SingletonMono<ArenaBounds>
    {
		public Vector3 Size => m_size;
		public Vector3 Extents => m_size / 2f;

        [SerializeField] private Vector2 m_size = new Vector2( 30, 18 );

#if UNITY_EDITOR
		[Header( "Editor / Tools" )]
		[SerializeField] private Color m_gizmoColor = Color.red;

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = m_gizmoColor;
			Gizmos.DrawWireCube( transform.position, m_size );
		}
#endif
	}
}
