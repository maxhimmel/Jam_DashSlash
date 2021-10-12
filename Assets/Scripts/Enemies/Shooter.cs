using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility;

namespace DashSlash.Gameplay.Enemies
{
    using Weapons;

    public class Shooter : Enemy
    {
		private LazyCachedChildComponent<Gun> m_gun = new LazyCachedChildComponent<Gun>( false );

		protected override void OnAwokenFromSpawn()
		{
			base.OnAwokenFromSpawn();

			UpdateRotation();
			m_gun[this].StartFiring();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			m_gun[this].StopFiring();
		}
	}
}
