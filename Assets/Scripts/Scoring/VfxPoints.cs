using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DashSlash.Gameplay.Scoring
{
	[RequireComponent( typeof( TMP_Text ) )]
    public class VfxPoints : MonoBehaviour
    {
		public Vector2 Offset => m_offset;

		[SerializeField] private Vector2 m_offset = Vector2.up * 0.5f;

        private TMP_Text m_text;

		public void SetText( string text )
		{
			m_text.text = text;
		}

		private void Awake()
		{
			m_text = GetComponent<TMP_Text>();
		}
	}
}
