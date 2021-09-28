using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;

namespace DashSlash.Gameplay.Movement
{
    public class WaveMovement : MonoBehaviour
    {
		[SerializeField] private Vector3 m_localDirection = Vector3.right;
		[SerializeField] private WaveDatum m_wave = new WaveDatum( 1, 1, 0 );

        private IWaveEvaluator m_evaluator;

		private void FixedUpdate()
		{
			transform.localPosition = m_localDirection * m_evaluator.Evaluate( m_wave, Time.timeSinceLevelLoad );
		}

		private void Awake()
		{
			m_evaluator = GetComponentInChildren<IWaveEvaluator>();
		}
	}
}
