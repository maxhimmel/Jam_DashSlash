using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies
{
    public class Hive : DirectionMover
    {
		private bool IsExpired => m_lifetimeExpiration > 0 && m_lifetimeExpiration <= Time.timeSinceLevelLoad;

		[Header( "Hive" )]
		[SerializeField] private float m_lifetime = 5;

		private float m_lifetimeExpiration = -1;

		protected override void UpdateState()
		{
			base.UpdateState();

			if ( IsExpired )
			{
				Destroy( gameObject );
			}
		}

		protected override void OnAwokenFromSpawn()
		{
			base.OnAwokenFromSpawn();

			m_lifetimeExpiration = m_lifetime + Time.timeSinceLevelLoad;
		}

		protected override void OnSliced( object sender, EventArgs e )
		{
			base.OnSliced( sender, e );

			m_lifetimeExpiration = -1;
		}

		protected override void OnDied()
		{
			base.OnDied();

			if ( IsExpired )
			{
				// Spawn Swarmers w/higher move/turn speeds
				// ...
			}
			else
			{
				// Spawn Swarmers normally?
					// Or, spawn them slightly dazed?
			}
		}
	}
}
