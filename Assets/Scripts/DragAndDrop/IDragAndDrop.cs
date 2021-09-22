using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay
{
    public interface IDragAndDrop
	{
		public event EventHandler<DragArgs> DragStarted;
		public event EventHandler<DragArgs> DragUpdated;
		public event EventHandler<DragArgs> DragReleased;
    }

    public class DragArgs : EventArgs
    {
        public Vector3 Normalized => Vector.normalized;
        public Vector3 Vector => End - Start;

        public Vector3 Start;
        public Vector3 End;

        public DragArgs( Vector3 start, Vector3 end )
        {
            Start = start;
            End = end;
        }
    }
}