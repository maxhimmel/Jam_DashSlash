using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Xam.Utility;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.UI
{
	using Utility;

	public class PickupMeterWidget : MonoBehaviour
	{
		private ScoreController Score => ScoreController.Instance;

		[Header( "Animation" )]
		[SerializeField] private Ease m_meterFillTween = Ease.OutCubic;
		[SerializeField] private float m_meterFillDuration = 0.3f;
		[SerializeField] private ColorPalette m_meterPalette = default;

		[Space]
		[SerializeField] private Ease m_comboDropClearTween = Ease.Linear;
		[SerializeField] private float m_comboDropClearDuration = 0.9f;

		[Header( "Elements" )]
		[SerializeField] private ImageFillAnimator  m_meter1 = default;
        [SerializeField] private ImageFillAnimator m_meter2 = default;

		private ImageFillAnimator m_currentMeter;
		private int m_groupBonus = 1;
		private Coroutine m_clearBonusRoutine;

		private void OnPickupsUpdated( object sender, ScoreEventArgs e )
		{
			m_currentMeter.Fill( e.PickupRatio, m_meterFillDuration, m_meterFillTween );

			if ( m_groupBonus != e.PickupGroupBonus )
			{
				bool isFinalGroup = e.PickupGroupBonus <= 1;
				m_currentMeter.Fill( isFinalGroup ? 0 : 1, 0, m_meterFillTween );
				m_currentMeter.Image.color = m_meterPalette.GetColor( e.PickupGroupBonus - 2 );

				m_currentMeter = GetNextMeter();
				m_currentMeter.transform.SetAsLastSibling();
				m_currentMeter.Image.color = m_meterPalette.GetColor( e.PickupGroupBonus - 1 );

				if ( !isFinalGroup )
				{
					m_currentMeter.Fill( e.PickupRatio, 0, m_meterFillTween );
				}

				m_groupBonus = e.PickupGroupBonus;
			}
		}

		private void OnComboDropped( object sender, ScoreEventArgs e )
		{
			this.TryStopCoroutine( ref m_clearBonusRoutine );
			m_clearBonusRoutine = StartCoroutine( UpdateComboDropClearing() );
		}

		private IEnumerator UpdateComboDropClearing()
		{
			ImageFillAnimator prevMeter = null;
			float durationPerGroup = m_comboDropClearDuration / m_groupBonus;

			for ( int group = m_groupBonus; group >= 1; --group )
			{
				bool isFinalGroup = group <= 1;

				if ( prevMeter != null && !isFinalGroup )
				{
					prevMeter.Fill( 1, 0, m_comboDropClearTween );
					prevMeter.Image.color = m_meterPalette.GetColor( group - 2 );
				}

				m_currentMeter.Fill( 0, durationPerGroup, m_comboDropClearTween );
				yield return new WaitForSeconds( durationPerGroup );

				if ( isFinalGroup ) { break; }

				prevMeter = m_currentMeter;

				m_currentMeter = GetNextMeter();
				m_currentMeter.transform.SetAsLastSibling();
			}

			m_groupBonus = 1;
			m_clearBonusRoutine = null;
		}

		private ImageFillAnimator GetNextMeter()
		{
			return m_currentMeter == m_meter1
				? m_meter2
				: m_meter1;
		}

		private void Start()
		{
			m_meter1.Fill( 0, 0, m_meterFillTween );
			m_meter2.Fill( 0, 0, m_meterFillTween );

			m_meter1.transform.SetAsLastSibling();
			m_currentMeter = m_meter1;
			m_currentMeter.Image.color = m_meterPalette.GetColor( 0 );

			Score.PickupsUpdated += OnPickupsUpdated;
			Score.ComboDropped += OnComboDropped;
		}

		private void OnDestroy()
		{
			if ( ScoreController.Exists )
			{
				Score.PickupsUpdated -= OnPickupsUpdated;
				Score.ComboDropped -= OnComboDropped;
			}
		}
	}
}
