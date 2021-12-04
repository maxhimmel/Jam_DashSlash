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
	using Scoring;

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
		[SerializeField] private ImageFillAnimator m_meterPrefab = default;
		[SerializeField] private Transform m_meterContainer = default;

		private Dictionary<int, ImageFillAnimator> m_meters = new Dictionary<int, ImageFillAnimator>();
		private int m_groupBonus = 1;
		private Coroutine m_clearBonusRoutine;

		private void OnPickupsUpdated( object sender, ScoreEventArgs e )
		{
			if ( m_groupBonus != e.PickupGroupBonus )
			{
				var prevMeter = GetMeter( m_groupBonus - 1 );
				int fillAmount = m_groupBonus < e.PickupGroupBonus ? 1 : 0;
				prevMeter.Fill( fillAmount, m_meterFillDuration, m_meterFillTween );

				m_groupBonus = e.PickupGroupBonus;
			}

			var meter = GetMeter( e.PickupGroupBonus - 1 );
			meter.Fill( e.PickupRatio, m_meterFillDuration, m_meterFillTween );
		}

		private void OnComboDropped( object sender, ScoreEventArgs e )
		{
			if ( e.Pickups > 0 ) { return; }

			this.TryStopCoroutine( ref m_clearBonusRoutine );
			m_clearBonusRoutine = StartCoroutine( UpdateComboDropClearing() );
		}

		private IEnumerator UpdateComboDropClearing()
		{
			float durationPerGroup = m_comboDropClearDuration / m_groupBonus;

			for ( int group = m_groupBonus; group >= 1; --group )
			{
				var meter = GetMeter( group - 1 );
				meter.Fill( 0, durationPerGroup, m_comboDropClearTween );

				yield return new WaitForSeconds( durationPerGroup );
			}

			m_clearBonusRoutine = null;
		}

		private ImageFillAnimator GetMeter( int idx )
		{
			m_meters.TryGetValue( idx, out var meter );
			return meter;
		}

		private void Start()
		{
			InitMeters();

			Score.PickupsUpdated += OnPickupsUpdated;
			Score.ComboDropped += OnComboDropped;
		}

		private void InitMeters()
		{
			m_meterPrefab.gameObject.SetActive( false );

			for ( int idx = 0; idx < m_meterPalette.Count; ++idx )
			{
				var fillMeter = Instantiate( m_meterPrefab, m_meterContainer );
				fillMeter.gameObject.SetActive( true );
				fillMeter.name = $"Image_Meter{idx}";

				fillMeter.Fill( 0, 0, Ease.Unset );
				fillMeter.Image.color = m_meterPalette.GetColor( idx );

				m_meters.Add( idx, fillMeter );
			}
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
