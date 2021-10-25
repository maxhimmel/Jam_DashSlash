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
		[Header( "Pickup Meter" )]
		[SerializeField] private ImageFillAnimator m_pickupMeterElement = default;
		[SerializeField] private TMP_Text m_pickupGroupBonusElement = default;
		[SerializeField] private Ease m_pickupMeterFillTween = Ease.OutCubic;
		[SerializeField] private float m_pickupMeterFillDuration = 0.3f;

		[Header( "Elements" )]
		[SerializeField] private TMP_Text m_comboElement = default;
		[SerializeField] private TMP_Text m_bonusElement = default;
		[SerializeField] private TMP_Text m_scoreElement = default;

		private ScoreController Score => ScoreController.Instance;

		private void OnScoreUpdated( object sender, System.EventArgs e )
		{
			ScoreController score = sender as ScoreController;

			m_comboElement.text = score.ComboSlices.ToString();
			m_comboElement.enabled = true;

			m_bonusElement.text = score.GetComboBonus().ToString();
			m_bonusElement.gameObject.SetActive( true );

			m_scoreElement.text = score.Score.ToString();

			m_pickupGroupBonusElement.text = score.GetPickupGroupBonus().ToString();
			m_pickupMeterElement.Fill( score.GetPickupRatio(), m_pickupMeterFillDuration, m_pickupMeterFillTween );
		}

		private void OnComboDropped( object sender, System.EventArgs e )
		{
			ScoreController score = sender as ScoreController;

			m_comboElement.text = 0.ToString();
			m_comboElement.enabled = false;

			m_bonusElement.text = 0.ToString();
			m_bonusElement.gameObject.SetActive( false );

			m_pickupGroupBonusElement.text = score.GetPickupGroupBonus().ToString();
			m_pickupMeterElement.Fill( 0, m_pickupMeterFillDuration, m_pickupMeterFillTween, true );
		}

		private void Start()
		{
			m_comboElement.text = 0.ToString();
			m_comboElement.enabled = false;

			m_bonusElement.text = 0.ToString();
			m_bonusElement.gameObject.SetActive( false );

			m_scoreElement.text = 0.ToString();

			m_pickupGroupBonusElement.text = Score.GetPickupGroupBonus().ToString();
			m_pickupMeterElement.Fill( 0, 0, m_pickupMeterFillTween );

			Score.ScoreUpdated += OnScoreUpdated;
			Score.ComboDropped += OnComboDropped;
		}

		private void OnDestroy()
		{
			Score.ScoreUpdated -= OnScoreUpdated;
			Score.ComboDropped -= OnComboDropped;
		}
	}
}
