using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DashSlash.Gameplay.UI.Utility
{
    [RequireComponent( typeof( Image ) )]
    public class ImageFillAnimator : MonoBehaviour
    {
        private Image m_image;
		private Tweener m_fillTweener;

		public void Fill( float value, float duration, Ease ease, bool completeInteruption = false )
		{
			if ( m_fillTweener.IsActive() )
			{
				m_fillTweener.Kill( completeInteruption );
			}

			if ( duration > 0 )
			{
				m_fillTweener = m_image.DOFillAmount( value, duration )
					.SetEase( ease );
			}
			else
			{
				m_image.fillAmount = value;
			}
		}

		private void Awake()
		{
			m_image = GetComponent<Image>();
		}
	}
}
