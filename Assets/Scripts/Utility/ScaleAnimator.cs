using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Utility
{
    public class ScaleAnimator : MonoBehaviour
    {
		public bool IsPlaying => m_scaleRoutine != null;

        [SerializeField] private bool m_playOnStart = true;
		[SerializeField] private Vector3 m_axisScalar = Vector3.one;
        [SerializeField] private AnimationCurve m_curve = AnimationCurve.Linear( 0, 0, 1, 1 );

		[Space]
		[SerializeField] private float m_duration = 1;
		[SerializeField] private Vector3 m_size = Vector3.one * 3;

		private Coroutine m_scaleRoutine;

		private void Start()
		{
			if ( m_playOnStart )
			{
				Play( m_duration, m_size );
			}
		}

		public void Play( float duration, Vector3 size )
		{
			if ( IsPlaying )
			{
				this.TryStopCoroutine( ref m_scaleRoutine );
			}

			m_scaleRoutine = StartCoroutine( UpdateScaling( duration, size ) );
		}

		private IEnumerator UpdateScaling( float duration, Vector3 size )
		{
			Vector3 start = transform.localScale;
			float timer = 0;

			while ( timer < 1 )
			{
				timer += Time.deltaTime / duration;
				timer = Mathf.Min( timer, 1 );

				float lerp = m_curve.Evaluate( timer );
				
				Vector3 newSize = Vector3.LerpUnclamped( start, size, lerp );
				newSize = VectorExtensions.Multiply( newSize, m_axisScalar );

				transform.localScale = newSize;
				yield return null;
			}

			m_scaleRoutine = null;
		}
    }
}
