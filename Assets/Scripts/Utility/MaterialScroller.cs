using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Vfx
{
    [RequireComponent( typeof( Renderer ) )]
    public class MaterialScroller : MonoBehaviour
    {
		[SerializeField] private string m_textureName = "_MainTex";
		[SerializeField] private Vector2 m_scrollSpeed = Vector2.zero;

        private Renderer m_renderer;
		private Vector2 m_offset;

		private void Update()
		{
			m_offset += m_scrollSpeed * Time.deltaTime;
			m_renderer.material.SetTextureOffset( m_textureName, m_offset );
		}

		private void Awake()
		{
			m_renderer = GetComponent<Renderer>();
		}
	}
}
