using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay
{
	[System.Serializable]
	public class ScoreController
	{
		private const int k_comboBase = 2;

		public event System.EventHandler ScoreUpdated;
		public event System.EventHandler ComboDropped;

		public int Score { get; private set; }
		public int Pickups { get; private set; }
		public int ComboSlices { get; private set; }

		private bool m_hasKills = false;
		private bool m_hasPickups = false;

		public void BeginCombo()
		{
			m_hasKills = false;
			m_hasPickups = false;

			this.Log( "Begin combo", Colors.Lime );
		}

		public bool TryClearCombo()
		{
			if ( !m_hasKills && !m_hasPickups )
			{
				ComboSlices = 0;

				this.Log( "Cleared combo", Colors.Magenta );
				ComboDropped?.Invoke( this, System.EventArgs.Empty );

				return true;
			}
			return false;
		}

		public void AddPickup()
		{
			++Pickups;
			m_hasPickups = true;

			this.Log( $"Pickups : {Pickups}", Colors.Yellow );
			ScoreUpdated?.Invoke( this, System.EventArgs.Empty );
		}

		public void AddSliceKill()
		{
			int comboBonus = GetComboBonus();

			++ComboSlices;
			Score += ComboSlices * comboBonus;

			m_hasKills = true;

			this.Log( $"Sliced! {Score} : " +
				$" added({ComboSlices * comboBonus})" +
				$" <b>|</b> combo({ComboSlices})" +
				$" <b>|</b> bonus({comboBonus})",
				Colors.Olive );
			ScoreUpdated?.Invoke( this, System.EventArgs.Empty );
		}

		public int GetComboBonus()
		{
			return (int)Mathf.Pow( k_comboBase, ComboSlices );
		}
	}
}
