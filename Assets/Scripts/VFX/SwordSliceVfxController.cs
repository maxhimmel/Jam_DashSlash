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
		[SerializeField] private float m_slomoDuration = 0.25f;

		private Coroutine m_slomoRoutine;
		private CinemachineBrain m_cineBrain;

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

			this.TryStopCoroutine( ref m_slomoRoutine );
			m_slomoRoutine = this.StartWaitingForUnscaledSeconds( m_slomoDuration, () =>
			{
				ClearSlomo();
				StopCameraState( "Slice" );
			} );
		}

		private void ClearSlomo()
		{
			this.TryStopCoroutine( ref m_slomoRoutine );
			TimeManager.Instance.SetTimeScale( 1, m_slomoRampSpeed );
		}

		private void PlayCameraState( string state )
		{
			CameraAnimator.SetBool( state, true );
		}

		private void StopCameraState( string state )
		{
			CameraAnimator.SetBool( state, false );
		}

		private void Start()
		{
			m_cineBrain = CinemachineCore.Instance.GetActiveBrain( 0 );
		}
	}
}
