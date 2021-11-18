using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;
using Xam.Utility.Extensions;

namespace DashSlash.Gameplay.Player.Animation
{
    using Vfx.Googly;

    public class PlayerGooglyEyesController : MonoBehaviour
    {
		private bool IsGazing => m_gazeRoutine != null;

		[SerializeField] private float m_gazeDuration = 1;

        private GooglyEyesController m_googlyEyes;
		private Rigidbody2DBucket m_poiBucket;
		private Rigidbody2D m_currentPoi;
		private Coroutine m_gazeRoutine;
		private float m_changeGazeTargetEndTime;

		private void OnPoiEntered( object sender, Rigidbody2D e )
		{
			if ( !IsGazing )
			{
				m_currentPoi = GetClosestPoi();
				m_changeGazeTargetEndTime = Time.timeSinceLevelLoad + m_gazeDuration;

				m_gazeRoutine = StartCoroutine( UpdateGaze() );
			}
		}

		private IEnumerator UpdateGaze()
		{
			while ( enabled )
			{
				if ( m_currentPoi == null || m_changeGazeTargetEndTime <= Time.timeSinceLevelLoad )
				{
					m_currentPoi = GetClosestPoi();
					m_changeGazeTargetEndTime = Time.timeSinceLevelLoad + m_gazeDuration;
				}

				if ( m_currentPoi == null ) { break; }

				Vector3 selfToPoi = m_currentPoi.position - transform.position.VectorXY();
				m_googlyEyes.SetDesiredLookDirection( selfToPoi );

				yield return null;
			}

			m_googlyEyes.SetDesiredLookDirection( Vector3.zero );

			m_gazeRoutine = null;
		}

		private void OnPoiExited( object sender, Rigidbody2D e )
		{
			if ( e == m_currentPoi )
			{
				m_currentPoi = GetClosestPoi();
				m_changeGazeTargetEndTime = Time.timeSinceLevelLoad + m_gazeDuration;
			}
		}

		private Rigidbody2D GetClosestPoi()
		{
			float closestDistSqr = Mathf.Infinity;
			Rigidbody2D result = null;

			foreach ( var poi in m_poiBucket.Targets )
			{
				if ( !CanLookAtPoi( poi ) ) { continue; }

				Vector3 selfToPoi = poi.position - transform.position.VectorXY();
				float distSqr = selfToPoi.sqrMagnitude;

				if ( distSqr < closestDistSqr )
				{
					closestDistSqr = distSqr;
					result = poi;
				}
			}

			return result;
		}

		private bool CanLookAtPoi( Rigidbody2D poi )
		{
			// TODO: How can we clean out destroyed gameobjects which are still cached by the buckets?
			if ( poi == null ) { return false; }

			var poiPlayer = poi.GetComponentInParent<PlayerController>();
			if ( poiPlayer != null ) { return false; }

			return true;
		}

		public void PlayDazedAnim( float duration )
		{
			m_googlyEyes.PlayDazedAnim( duration );
		}

		private void Update()
		{
			if ( Input.GetKeyDown( KeyCode.Backspace ) )
			{
				PlayDazedAnim( m_gazeDuration );
			}
		}

		private void Start()
		{
			m_poiBucket.TargetEntered += OnPoiEntered;
			m_poiBucket.TargetExited += OnPoiExited;
		}

		private void Awake()
		{
			m_googlyEyes = GetComponentInChildren<GooglyEyesController>();
			m_poiBucket = GetComponentInChildren<Rigidbody2DBucket>();
		}
	}
}
