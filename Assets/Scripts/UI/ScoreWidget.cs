using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.UI
{
	using Utility;

    public class ScoreWidget : MonoBehaviour
	{
		private ScoreController Score => ScoreController.Instance;

		[Header( "Pickup Meter" )]
		[SerializeField] private TMP_Text m_pickupGroupBonusElement = default;

		[Header( "Score Increment" )]
		[SerializeField] private TextCountAnimator m_scoreIncrementElement = default;
		[SerializeField] private float m_scoreIncrementDuration = 1;

		[Header( "Total Score" )]
		[SerializeField] private TextCountAnimator m_scoreElement = default;
		[SerializeField] private float m_scoreUpdateDuration = 1;

		[Header( "Elements" )]
		[SerializeField] private TMP_Text m_comboElement = default;
		[SerializeField] private TMP_Text m_bonusElement = default;

		private Coroutine m_scoreIncrementDisableRoutine;

		private void OnScoreUpdated( object sender, ScoreEventArgs e )
		{
			m_comboElement.text = e.ComboSlices.ToString();
			m_comboElement.enabled = true;

			m_bonusElement.text = e.ComboBonus.ToString();
			m_bonusElement.gameObject.SetActive( true );

			UpdateScoreIncrement( e.ScoreIncrement );

			m_scoreElement.SetValue( e.Score, m_scoreUpdateDuration, false );
		}

		private void OnPickupsUpdated( object sender, ScoreEventArgs e )
		{
			m_pickupGroupBonusElement.text = e.PickupGroupBonus.ToString();
		}

		private void OnComboDropped( object sender, ScoreEventArgs e )
		{
			m_comboElement.text = 0.ToString();
			m_comboElement.enabled = false;

			m_bonusElement.text = 0.ToString();
			m_bonusElement.gameObject.SetActive( false );

			UpdateScoreIncrement( e.ScoreIncrement );

			m_pickupGroupBonusElement.text = 1.ToString();
		}

		private void UpdateScoreIncrement( int scoreIncrement )
		{
			m_scoreIncrementElement.gameObject.SetActive( true );
			m_scoreIncrementElement.SetValue( scoreIncrement, 0, false );
			m_scoreIncrementElement.SetValue( 0, m_scoreIncrementDuration, true );

			this.TryStopCoroutine( ref m_scoreIncrementDisableRoutine );
			m_scoreIncrementDisableRoutine = this.StartWaitingForSeconds( 
				m_scoreIncrementDuration, m_scoreIncrementElement.gameObject.SetActive, false 
			);
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
