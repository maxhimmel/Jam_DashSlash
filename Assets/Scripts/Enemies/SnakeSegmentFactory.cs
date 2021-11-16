using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSlash.Gameplay.Enemies.Factories
{
    public class SnakeSegmentFactory : MonoBehaviour
    {
        [SerializeField] private int m_segmentCount = 6;
        [SerializeField] private SnakeSegment m_segmentPrefab = default;

        public List<SnakeSegment> CreateSegments( SnakeSegment head )
		{
            Vector3 spawnPos = head.transform.position;
            Quaternion spawnRot = head.transform.rotation;
            Transform parent = head.transform.parent;

            Vector3 spawnOffset = -head.transform.up * head.OffsetDistance;

            var nextSegment = head;
            
            List<SnakeSegment> results = new List<SnakeSegment>( m_segmentCount + 1 );
            results.Add( head );

            for ( int idx = 0; idx < m_segmentCount; ++idx )
			{
                spawnPos += spawnOffset;
                
                var newSegment = Instantiate( m_segmentPrefab, spawnPos, spawnRot, parent );
                newSegment.name = newSegment.name.Replace( "Clone", $" {idx} " );
                newSegment.gameObject.SetActive( true );

                newSegment.SetNextSegment( nextSegment );
                nextSegment = newSegment;

                results.Add( newSegment );
			}

            return results;
		}
    }
}
