using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;

namespace DashSlash.Gameplay.Enemies
{
    using Weapons;

    public class Shooter : MonoBehaviour
    {
		private LazyCachedChildComponent<Gun> m_gun = new LazyCachedChildComponent<Gun>( false );

		private void OnEnable()
		{
			m_gun[this].StartFiring();
		}

		private void OnDisable()
		{
			m_gun[this].StopFiring();
		}
	}
}
