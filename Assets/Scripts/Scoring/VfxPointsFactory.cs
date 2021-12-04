using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay.Patterns;

namespace DashSlash.Gameplay.Scoring
{
    public class VfxPointsFactory : Factory<VfxPoints>
    {
		public override VfxPoints Create( Vector3 position = default, Quaternion rotation = default, Transform parent = null )
		{
			Vector3 offset = m_prefab.Offset;
			return base.Create( position + offset, rotation, parent );
		}
	}
}
