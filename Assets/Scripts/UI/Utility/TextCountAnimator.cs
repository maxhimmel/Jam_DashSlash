using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DashSlash.Gameplay.UI
{
    [RequireComponent( typeof( TMP_Text ) )]
    public class TextCountAnimator : MonoBehaviour
    {
        [SerializeField] private string m_format = "{0}";

        private TMP_Text m_text;
        private Tweener m_countTweener;
        private int m_prevValue = 0;

        public void SetFormat( string format )
		{
            m_format = string.IsNullOrEmpty( format )
                ? "{0}"
                : format;
		}

        public void SetValue( int value, float duration, bool completeInterruption = false, params object[] extraFormatArgs )
        {
            if ( m_countTweener.IsActive() )
			{
                m_countTweener.Kill( completeInterruption );
			}

            if ( duration > 0 )
            {
                int counter = m_prevValue;
                m_countTweener = DOTween.To( () => counter, tweenIncrement => counter = tweenIncrement, value, duration )
                    .OnUpdate( () =>
                    {
                        m_prevValue = counter;
                        m_text.text = string.Format( m_format, counter, extraFormatArgs );
                    } )
                    .OnComplete( () =>
                    {
                        m_text.text = string.Format( m_format, value, extraFormatArgs );
                    } );
            }
            else
			{
                m_prevValue = value;
                m_text.text = string.Format( m_format, value, extraFormatArgs );
            }
        }

		private void Awake()
		{
            m_text = GetComponent<TMP_Text>();
		}
	}
}
