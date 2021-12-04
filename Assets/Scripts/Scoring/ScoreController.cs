using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Scoring
{
	[System.Serializable]
	public class ScoreController : SingletonMono<ScoreController>
	{
		private const int k_comboBase = 2;

		public event System.EventHandler<ScoreEventArgs> ScoreUpdated;
		public event System.EventHandler<ScoreEventArgs> PickupsUpdated;
		public event System.EventHandler<ScoreEventArgs> ComboDropped;

		public int MaxPickupGroupBonus => m_maxPickupGroupBonus;

		[SerializeField] private bool m_enableLogging = false;

		[Header( "Scoring" )]
		[Tooltip( "The rate at which the meter is constantly decaying." )]
		[SerializeField, Min( 0 )] private float m_meterDecayRate = 1;
		[SerializeField, Min( 0 )] private float m_meterDecayValue = 1;
		[Tooltip( "How much the meter is reduced when missing a dash/slash." )]
		[SerializeField, Min( 0 )] private float m_meterUsageCost = 0.5f;

		[Header( "Pickups" )]
		[SerializeField, Min( 0 )] private int m_maxPickupGroupBonus = 5;
		[SerializeField, Min( 0 )] private int m_pickupGroupCount = 10;

		public int Score { get; private set; }
		public float Pickups { get; private set; }
		public int ComboSlices { get; private set; }

		private bool m_hasKills = false;
		private bool m_hasPickups = false;
		private bool m_canScorePickups = true;
		private Coroutine m_meterDecayRoutine;

		public void SetPickupScoringActive( bool isActive )
		{
			m_canScorePickups = isActive;
		}

		public void BeginCombo()
		{
			m_hasKills = false;
			m_hasPickups = false;

			Log( "Begin combo", Colors.Lime );
		}

		public bool TryClearBonus()
		{
			if ( !m_hasKills && !m_hasPickups )
			{
				ClearCombo( false );
				return true;
			}
			return false;
		}

		public void ForceClearCombo()
		{
			ClearCombo( true );

			m_hasKills = false;
			m_hasPickups = false;
		}

		private void ClearCombo( bool isForced )
		{
			int scoreIncrement = 0;
			bool hadCombo = ComboSlices > 0;
			bool hadPickups = Pickups > 0;
			bool canDrop = hadCombo && GetPickupGroupBonus() <= 1;

			// Apply and clear bonus ...
			if ( canDrop )
			{
				scoreIncrement = ApplyBonusCleared();
			}

			// Update pickups ...
			if ( hadPickups )
			{
				float pickups = isForced ? 0 : Pickups - m_meterUsageCost;
				SetPickups( pickups );
			}

			// Finally, apply combo drop ...
			if ( isForced || canDrop )
			{
				Log( "Cleared combo", Colors.Red );
				ComboDropped?.Invoke( this, new ScoreEventArgs()
				{
					Score = Score,
					ScoreIncrement = scoreIncrement,
					ComboSlices = ComboSlices,
					ComboBonus = GetComboBonus(),
					Pickups = Pickups,
					PickupGroupBonus = GetPickupGroupBonus(),
					PickupRatio = GetPickupRatio( true ),
				} );
			}
		}

		private int ApplyBonusCleared()
		{
			int scoreIncrement = ComboSlices * GetComboBonus() * GetPickupGroupBonus();
			Score += scoreIncrement;

			ScoreUpdated?.Invoke( this, new ScoreEventArgs()
			{
				Score = Score,
				ScoreIncrement = scoreIncrement,
				ComboSlices = ComboSlices,
				ComboBonus = GetComboBonus(),
				Pickups = Pickups,
				PickupGroupBonus = GetPickupGroupBonus(),
				PickupRatio = GetPickupRatio( true ),
			} );

			ComboSlices = 0;

			return scoreIncrement;
		}

		public void AddPickup()
		{
			m_hasPickups = true;

			Log( $"Pickups : {Pickups + 1}", Colors.Yellow );
			SetPickups( Pickups + 1 );
		}

		public int AddSliceKill()
		{
			int comboBonus = GetComboBonus();

			++ComboSlices;

			int scoreIncrement = ComboSlices * comboBonus;
			Score += scoreIncrement;

			m_hasKills = true;

			Log( $"Sliced! {Score} :" +
				$" added({ComboSlices * comboBonus})" +
				$" <b>|</b> combo({ComboSlices})" +
				$" <b>|</b> bonus({comboBonus})",
				Colors.Olive );

			ScoreUpdated?.Invoke( this, new ScoreEventArgs()
			{
				Score = Score,
				ScoreIncrement = scoreIncrement,
				ComboSlices = ComboSlices,
				ComboBonus = GetComboBonus(),
				Pickups = Pickups,
				PickupGroupBonus = GetPickupGroupBonus(),
				PickupRatio = GetPickupRatio( true ),
			} );

			return scoreIncrement;
		}

		public int AddSliceKill( int baseScore )
		{
			++ComboSlices;

			int scoreIncrement = ComboSlices * baseScore;
			Score += scoreIncrement;

			m_hasKills = true;

			Log( $"Sliced! {Score} :" +
				$" added({ComboSlices * baseScore})" +
				$" <b>|</b> combo({ComboSlices})",
				Colors.Olive );

			ScoreUpdated?.Invoke( this, new ScoreEventArgs()
			{
				Score = Score,
				ScoreIncrement = scoreIncrement,
				ComboSlices = ComboSlices,
				ComboBonus = GetComboBonus(),
				Pickups = Pickups,
				PickupGroupBonus = GetPickupGroupBonus(),
				PickupRatio = GetPickupRatio( true ),
			} );

			return scoreIncrement;
		}

		private int GetComboBonus()
		{
			return (int)Mathf.Pow( k_comboBase, ComboSlices );
		}

		private int GetPickupGroupBonus()
		{
			int result = 1 + Mathf.FloorToInt( Pickups / m_pickupGroupCount );
			return Mathf.Min( result, m_maxPickupGroupBonus );
		}

		private float GetPickupRatio( bool isWrapped )
		{
			if ( Pickups / m_pickupGroupCount >= m_maxPickupGroupBonus ) { return 1; }

			float pickups = isWrapped
				? Pickups % m_pickupGroupCount
				: Pickups;

			return pickups / (float)m_pickupGroupCount;
		}

		private void SetPickups( float pickups, bool sendEvent = true  )
		{
			if ( !m_canScorePickups ) { return; }

			float prevPickups = Pickups;
			float maxPickups = m_maxPickupGroupBonus * m_pickupGroupCount;
			Pickups = Mathf.Clamp( pickups, 0, maxPickups );

			if ( prevPickups == Pickups ) { return; }
			if ( !sendEvent ) { return; }

			PickupsUpdated?.Invoke( this, new ScoreEventArgs()
			{
				Score = Score,
				ScoreIncrement = 0,
				ComboSlices = ComboSlices,
				ComboBonus = GetComboBonus(),
				Pickups = Pickups,
				PickupGroupBonus = GetPickupGroupBonus(),
				PickupRatio = GetPickupRatio( true ),
			} );
		}

		private void OnDisable()
		{
			this.TryStopCoroutine( ref m_meterDecayRoutine );
		}

		private void OnEnable()
		{
			m_meterDecayRoutine = StartCoroutine( UpdateMeterDecay() );
		}

		private IEnumerator UpdateMeterDecay()
		{
			while ( enabled )
			{
				yield return new WaitForSeconds( m_meterDecayRate );

				if ( Pickups > 0 )
				{
					SetPickups( Pickups - m_meterDecayValue );
				}
			}

			m_meterDecayRoutine = null;
		}

		private void Log( string message, Colors color = Colors.Black )
		{
			if ( !m_enableLogging ) { return; }
			LogExtensions.Log( this, message, color );
		}
	}

	public class ScoreEventArgs : System.EventArgs
	{
		public int Score;
		public int ScoreIncrement;

		public int ComboSlices;
		public int ComboBonus;

		public float Pickups;
		public int PickupGroupBonus;
		public float PickupRatio;
	}
}
