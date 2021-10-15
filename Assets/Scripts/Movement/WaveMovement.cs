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
		private float m_timer;

		public void SetLocalDirection( Vector3 direction )
		{
			m_localDirection = direction;
		}

		public void SetWaveData( WaveDatum datum )
		{
			m_wave = datum;
		}

		private void FixedUpdate()
		{
			m_timer += Time.deltaTime;
			transform.localPosition = m_localDirection * m_evaluator.Evaluate( m_wave, m_timer );
		}

		private void Awake()
		{
			m_evaluator = GetComponentInChildren<IWaveEvaluator>();
		}
	}
}
