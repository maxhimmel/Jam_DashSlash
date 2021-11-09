using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;
using Xam.Utility.Extensions;
using Cinemachine;

namespace DashSlash
{
    public class SwordSliceVfxController : MonoBehaviour
	{
		private Animator CameraAnimator => StateCamera.m_AnimatedTarget;
		private CinemachineStateDrivenCamera StateCamera => m_cineBrain.ActiveVirtualCamera as CinemachineStateDrivenCamera;

		[Header( "Slomo" )]
		[SerializeField] private float m_slomoScale = 0.3f;
		[SerializeField] private float m_slomoRampSpeed = 0.1f;

		[Header( "Duration" )]
		[SerializeField] private float m_minSlomoDuration = 0.08f;
		[SerializeField] private float m_maxSlomoDuration = 0.2f;
		[SerializeField] private float m_slomoDurationStep = 0.03f;

		private Coroutine m_slomoRoutine;
		private CinemachineBrain m_cineBrain;
		private float m_slomoStepAmount;

		public void IncrementSlomoDuration()
		{
			m_slomoStepAmount += m_slomoDurationStep;
		}

		public void StopSliceVfx()
		{
			ClearSlomo();
			StopCameraState( "Slice" );
		}

		public void PlaySliceVfx()
		{
			BeginSlomo();
			PlayCameraState( "Slice" );
		}

		private void BeginSlomo()
		{
			TimeManager.Instance.SetTimeScale( m_slomoScale, m_slomoRampSpeed );

			float slomoDuration = Mathf.Clamp( m_slomoStepAmount + m_minSlomoDuration, m_minSlomoDuration, m_maxSlomoDuration );

			this.TryStopCoroutine( ref m_slomoRoutine );
			m_slomoRoutine = this.StartWaitingForUnscaledSeconds( slomoDuration, () =>
			{
				ClearSlomo();
				StopCameraState( "Slice" );
			} );
		}

		private void ClearSlomo()
		{
			m_slomoStepAmount = 0;

			this.TryStopCoroutine( ref m_slomoRoutine );
			TimeManager.Instance.SetTimeScale( 1, m_slomoRampSpeed );
		}

		private void StopCameraState( string state )
		{
			CameraAnimator.SetBool( state, false );
		}

		private void PlayCameraState( string state )
		{
			CameraAnimator.SetBool( state, true );
		}

		private void Start()
		{
			m_cineBrain = CinemachineCore.Instance.GetActiveBrain( 0 );
		}
	}
}
