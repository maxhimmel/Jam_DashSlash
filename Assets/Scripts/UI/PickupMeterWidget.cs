using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Xam.Utility;

namespace DashSlash.Gameplay.UI
{
	using Utility;

	public class PickupMeterWidget : MonoBehaviour
	{
		[Header( "Animation" )]
		[SerializeField] private Ease m_meterFillTween = Ease.OutCubic;
		[SerializeField] private float m_meterFillDuration = 0.3f;
		[SerializeField] private ColorPalette m_meterPalette = default;

		[Header( "Elements" )]
		[SerializeField] private ImageFillAnimator  m_meter1 = default;
        [SerializeField] private ImageFillAnimator m_meter2 = default;

		private ImageFillAnimator m_currentMeter;
		private int m_groupBonus = 1;

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

			ScoreController.Instance.PickupsUpdated += OnPickupsUpdated;
			ScoreController.Instance.ComboDropped += OnComboDropped;
		}

		private void OnDestroy()
		{
			if ( ScoreController.Exists )
			{
				ScoreController.Instance.PickupsUpdated -= OnPickupsUpdated;
				ScoreController.Instance.ComboDropped -= OnComboDropped;
			}
		}
	}
}
