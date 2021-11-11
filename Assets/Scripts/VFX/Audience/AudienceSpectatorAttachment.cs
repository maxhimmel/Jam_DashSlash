using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace DashSlash.Vfx.Audiences
{
    [RequireComponent( typeof( AudienceSpectator ) )]
    public class AudienceSpectatorAttachment : DynamicPoolAttachment<AudienceSpectator>
    {
    }
}
