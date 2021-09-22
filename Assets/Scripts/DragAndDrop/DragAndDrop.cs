using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

using Xam.Gameplay.Cameras;

namespace DashSlash.Gameplay
{
    public class DragAndDrop : MonoBehaviour, IDragAndDrop
    {
        public event EventHandler<DragArgs> DragStarted;
        public event EventHandler<DragArgs> DragUpdated;
        public event EventHandler<DragArgs> DragReleased;

        public bool IsDragging => m_currentDrag != null;
        public DragArgs CurrentDrag => m_currentDrag;

        private Mouse Mouse => ReInput.controllers.Mouse;
        private Camera Camera => m_cameraFinder.GetCamera();

        private ICameraFinder m_cameraFinder;
        private DragArgs m_currentDrag;

		private void Update()
		{
            if ( Mouse.GetButtonDown( 0 ) )
			{
                Vector3 pos = GetMouseWorldPosition();
                m_currentDrag = new DragArgs( pos, pos );

                DragStarted?.Invoke( this, m_currentDrag );
			}
            else if ( Mouse.GetButtonUp( 0 ) )
			{
                m_currentDrag.End = GetMouseWorldPosition();

                DragReleased?.Invoke( this, m_currentDrag );

                m_currentDrag = null;
			}

            if ( IsDragging )
            {
                if ( Mouse.screenPositionDelta.sqrMagnitude > 0 )
                {
                    m_currentDrag.End = GetMouseWorldPosition();
                    DragUpdated?.Invoke( this, m_currentDrag );
                }
            }
		}

        public Vector3 GetMouseWorldPosition()
		{
            Vector2 screenPos = Mouse.screenPosition;
            float depth = transform.position.z - Camera.transform.position.z;

            Vector3 posInput = new Vector3( screenPos.x, screenPos.y, depth );
            return Camera.ScreenToWorldPoint( posInput );
		}

		private void Awake()
        {
            m_cameraFinder = GetComponentInParent<ICameraFinder>();
		}
	}
}