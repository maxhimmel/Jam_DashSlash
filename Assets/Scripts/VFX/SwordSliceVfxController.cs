using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;
using Xam.Utility.Extensions;
using UnityEngine.Rendering.PostProcessing;

namespace DashSlash
{
    public class SwordSliceVfxController : MonoBehaviour
	{
		[Header( "Slomo" )]
		[SerializeField] private float m_slomoScale = 0.3f;
		[SerializeField] private float m_slomoRampSpeed = 0.1f;
		[SerializeField] private float m_slomoDuration = 0.25f;

		private Coroutine m_slomoRoutine;

		public void StopSliceVfx()
		{
			ClearSlomo();
		}

		public void PlaySliceVfx()
		{
			BeginSlomo();
		}

		private void BeginSlomo()
		{
			TimeManager.Instance.SetTimeScale( m_slomoScale, m_slomoRampSpeed );

			this.TryStopCoroutine( ref m_slomoRoutine );
			m_slomoRoutine = this.StartWaitingForUnscaledSeconds( m_slomoDuration, () =>
			{
				ClearSlomo();
			} );
		}

		private void ClearSlomo()
		{
			this.TryStopCoroutine( ref m_slomoRoutine );
			TimeManager.Instance.SetTimeScale( 1, m_slomoRampSpeed );
		}
	}
}
