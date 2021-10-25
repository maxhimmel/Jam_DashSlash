using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DashSlash.Gameplay.UI
{
    public class ScoreWidget : MonoBehaviour
	{
		[SerializeField] private TMP_Text m_comboElement = default;
		[SerializeField] private TMP_Text m_bonusElement = default;
		[SerializeField] private TMP_Text m_scoreElement = default;
		[SerializeField] private Image m_pickupMeterElement = default;

		private ScoreController Score => ScoreController.Instance;

		private void OnScoreUpdated( object sender, System.EventArgs e )
		{
			ScoreController score = sender as ScoreController;

			m_comboElement.text = score.ComboSlices.ToString();
			m_comboElement.enabled = true;

			m_bonusElement.text = score.GetComboBonus().ToString();
			m_bonusElement.gameObject.SetActive( true );

			m_scoreElement.text = score.Score.ToString();
			m_pickupMeterElement.fillAmount = score.GetPickupRatio();
		}

		private void OnComboDropped( object sender, System.EventArgs e )
		{
			ScoreController score = sender as ScoreController;

			m_comboElement.text = 0.ToString();
			m_comboElement.enabled = false;

			m_bonusElement.text = 0.ToString();
			m_bonusElement.gameObject.SetActive( false );

			m_pickupMeterElement.fillAmount = 0;
		}

		private void Start()
		{
			m_comboElement.text = 0.ToString();
			m_comboElement.enabled = false;

			m_bonusElement.text = 0.ToString();
			m_bonusElement.gameObject.SetActive( false );

			m_scoreElement.text = 0.ToString();
			m_pickupMeterElement.fillAmount = 0;

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
