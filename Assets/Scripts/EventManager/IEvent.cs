using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.EventQueues
{
    public interface IEvent
    {
        PlayState State { get; }

        void Play();
    }

    public enum PlayState
    {
        Sleeping,
        Playing,
        Done
    }
}
