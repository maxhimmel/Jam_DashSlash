using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace DashSlash.Gameplay.UI
{
	using Utility;

    public class ScoreWidget : MonoBehaviour
	{
		private ScoreController Score => ScoreController.Instance;

		[Header( "Pickup Meter" )]
		[SerializeField] private ImageFillAnimator m_pickupMeterElement = default;
		[SerializeField] private TMP_Text m_pickupGroupBonusElement = default;
		[SerializeField] private Ease m_pickupMeterFillTween = Ease.OutCubic;
		[SerializeField] private float m_pickupMeterFillDuration = 0.3f;

		[Header( "Score Increment" )]
		[SerializeField] private TextCountAnimator m_scoreIncrementElement = default;
		[SerializeField] private float m_scoreIncrementDuration = 1;

		[Header( "Total Score" )]
		[SerializeField] private TextCountAnimator m_scoreElement = default;
		[SerializeField] private float m_scoreUpdateDuration = 1;

		[Header( "Elements" )]
		[SerializeField] private TMP_Text m_comboElement = default;
		[SerializeField] private TMP_Text m_bonusElement = default;

		private void OnScoreUpdated( object sender, ScoreEventArgs e )
		{
			m_comboElement.text = e.ComboSlices.ToString();
			m_comboElement.enabled = true;

			m_bonusElement.text = e.ComboBonus.ToString();
			m_bonusElement.gameObject.SetActive( true );

			m_scoreIncrementElement.gameObject.SetActive( true );
			m_scoreIncrementElement.SetValue( e.ScoreIncrement, 0 );
			m_scoreIncrementElement.SetValue( 0, m_scoreIncrementDuration );

			m_scoreElement.SetValue( e.Score, m_scoreUpdateDuration, false );
		}

		private void OnPickupsUpdated( object sender, ScoreEventArgs e )
		{
			m_pickupGroupBonusElement.text = e.PickupGroupBonus.ToString();
			m_pickupMeterElement.Fill( e.PickupRatio, m_pickupMeterFillDuration, m_pickupMeterFillTween );
		}

		private void OnComboDropped( object sender, ScoreEventArgs e )
		{
			m_comboElement.text = 0.ToString();
			m_comboElement.enabled = false;

			m_bonusElement.text = 0.ToString();
			m_bonusElement.gameObject.SetActive( false );

			m_scoreIncrementElement.SetValue( e.ScoreIncrement, 0, false );
			m_scoreIncrementElement.SetValue( 0, m_scoreIncrementDuration, true );
			// TODO: And then we gotta hide this element once it's back to zero
				 // ...

			m_pickupGroupBonusElement.text = 1.ToString();
			m_pickupMeterElement.Fill( 0, m_pickupMeterFillDuration, m_pickupMeterFillTween, true );
		}

		private void Start()
		{
			m_comboElement.text = 0.ToString();
			m_comboElement.enabled = false;

			m_bonusElement.text = 0.ToString();
			m_bonusElement.gameObject.SetActive( false );

			m_scoreIncrementElement.SetValue( 0, 0, true );
			m_scoreIncrementElement.gameObject.SetActive( false );

			m_scoreElement.SetValue( 0, 0, true );

			m_pickupGroupBonusElement.text = 1.ToString();
			m_pickupMeterElement.Fill( 0, 0, m_pickupMeterFillTween );

			Score.ScoreUpdated += OnScoreUpdated;
			Score.PickupsUpdated += OnPickupsUpdated;
			Score.ComboDropped += OnComboDropped;
		}

		private void OnDestroy()
		{
			if ( ScoreController.Exists )
			{
				Score.ScoreUpdated -= OnScoreUpdated;
				Score.PickupsUpdated -= OnPickupsUpdated;
				Score.ComboDropped -= OnComboDropped;
			}
		}
	}
}
