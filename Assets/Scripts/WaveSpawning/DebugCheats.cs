using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DashSlash.Gameplay.WaveSpawning
{
	using Enemies;
	using Enemies.Factories;

	public class DebugCheats : MonoBehaviour
    {
		private const float k_guiScale = 2;
		private readonly Vector2 k_nativeScreenSize = new Vector2( 1920, 1080 );

		[SerializeField] private List<EnemyKeyMap> m_enemyKeyMappings = new List<EnemyKeyMap>();

		[Header( "GUI" )]
		[SerializeField] private int m_columns = 3;

		private EnemySpawnEvent m_spawner;
		private EnemyFactory m_enemyFactory;

		private void Update()
		{
			foreach ( var mapping in m_enemyKeyMappings )
			{
				if ( Input.GetKeyDown( mapping.Key ) )
				{
					SpawnEnemy( mapping );
				}
			}
		}

		private void OnGUI()
		{
			Vector3 scale = k_guiScale * new Vector3( Screen.width / k_nativeScreenSize.x, Screen.height / k_nativeScreenSize.y, 1.0f );
			GUI.matrix = Matrix4x4.TRS( new Vector3( 0, 0, 0 ), Quaternion.identity, scale );

			GUILayout.BeginVertical();

			for ( int idx = 0; idx < m_enemyKeyMappings.Count; idx += m_columns )
			{
				GUILayout.BeginHorizontal();

				for ( int col = 0; col < m_columns; ++col )
				{
					int index = idx + col;
					if ( index >= m_enemyKeyMappings.Count ) { break; }

					var mapping = m_enemyKeyMappings[index];
					if ( GUILayout.Button( $"[{mapping.Key}] : <b>{mapping.Name}</b>" ) )
					{
						SpawnEnemy( mapping );
					}
				}

				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
		}

		private void SpawnEnemy( EnemyKeyMap mapping )
		{
			if ( !mapping.IsValid() )
			{
				Debug.LogException( new System.NullReferenceException( "EnemyKeyMap.Prefab" ) );
				return;
			}

			m_enemyFactory.SetEnemyPrefab( mapping.Prefab );
			m_spawner.Play();
		}

		private void Awake()
		{
			m_spawner = GetComponent<EnemySpawnEvent>();
			m_enemyFactory = GetComponent<EnemyFactory>();
		}

		[ButtonGroup( Order = -1 )]
		[Button( DirtyOnClick = true, Name = "Last --> Count" )]
		private void SetMappingKeyToNumber()
		{
			int lastIndex = m_enemyKeyMappings.Count - 1;
			if ( lastIndex < 0 ) { return; }

			var mapping = m_enemyKeyMappings[lastIndex];
			mapping.Key = IntToKeyCode( lastIndex + 1 );
		}

		[ButtonGroup( Order = -1 )]
		[Button( DirtyOnClick = true, Name = "All Keys --> Index" )]
		private void SetAllMappingKeysToIndex()
		{
			for ( int idx = 0; idx < m_enemyKeyMappings.Count; ++idx )
			{
				var mapping = m_enemyKeyMappings[idx];
				mapping.Key = IntToKeyCode( idx + 1 );
			}
		}

		private KeyCode IntToKeyCode( int number )
		{
			switch ( number )
			{
				default: return KeyCode.None;

				case 0: return KeyCode.Alpha0;
				case 1: return KeyCode.Alpha1;
				case 2: return KeyCode.Alpha2;
				case 3: return KeyCode.Alpha3;
				case 4: return KeyCode.Alpha4;
				case 5: return KeyCode.Alpha5;
				case 6: return KeyCode.Alpha6;
				case 7: return KeyCode.Alpha7;
				case 8: return KeyCode.Alpha8;
				case 9: return KeyCode.Alpha9;
			}
		}

		[System.Serializable]
        class EnemyKeyMap
		{
			public string Name => IsValid() ? Prefab.name : "MISSING REFERENCE";

			public KeyCode Key;
			public Enemy Prefab;

			public bool IsValid()
			{
				return Prefab != null;
			}
		}
	}
}
