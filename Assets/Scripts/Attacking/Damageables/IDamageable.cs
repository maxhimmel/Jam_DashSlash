using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay
{
    public interface IDamageable
    {
        void TakeDamage( DamageDatum dmgData );
    }

    [System.Serializable]
	public class DamageDatum
	{
        public Transform Instigator;
        public Transform DamageCauser;

        public Vector3 GetHitDirection( Vector3 otherPos )
		{
            return (otherPos - DamageCauser.position).normalized;
		}
	}

    [System.Serializable]
	public class IgnoreSlicingDamageDatum : DamageDatum
	{

	}

	public static class DamageableExtensions
	{
        public static bool TryGetDamageable<T>( this T self, out IDamageable damageable ) where T : Component
        {
            damageable = self != null
                ? self.GetComponent<IDamageable>()
                : null;

            return damageable != null;
        }
	}
}
