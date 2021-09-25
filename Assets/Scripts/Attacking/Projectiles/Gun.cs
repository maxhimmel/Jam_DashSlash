using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;
using Xam.Gameplay.Patterns;

namespace DashSlash.Gameplay.Weapons
{
    public class Gun : MonoBehaviour
    {
        public bool IsFiring => m_firingRoutine != null;

        [SerializeField] private ProjectileFactory m_projectileFactory = default;

        [Space]
        [SerializeField] private float m_fireRate = 1.3f;
        [SerializeField] private float m_projectileSpeed = 4;

        private Coroutine m_firingRoutine;

        public void StartFiring()
		{
            if ( IsFiring ) { return; }

            m_firingRoutine = StartCoroutine( UpdateFiring() );
		}

        private IEnumerator UpdateFiring()
		{
            float nextFireTime;

            do
            {
                Projectile projectile = CreateProjectile();
                projectile.Fire( m_projectileSpeed );

                nextFireTime = Time.timeSinceLevelLoad + m_fireRate;
                while ( nextFireTime > Time.timeSinceLevelLoad ) 
                {
                    yield return new WaitForFixedUpdate(); 
                }

            } while ( IsFiring );

            m_firingRoutine = null;
		}

        private Projectile CreateProjectile()
		{
            Vector3 spawnPos = m_projectileFactory.transform.position;
            Quaternion spawnRot = m_projectileFactory.transform.rotation;

            Projectile newProjectile = m_projectileFactory.Create( spawnPos, spawnRot );
            newProjectile.SetOwner( transform );

            return newProjectile;
		}

        public void StopFiring()
		{
            if ( IsFiring )
			{
                this.TryStopCoroutine( ref m_firingRoutine );
			}
		}
    }
}
