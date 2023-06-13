namespace Example
{
	using System;
	using UnityEngine;

	using Random = UnityEngine.Random;

	[ExecuteAlways]
	public sealed class VegetationSpawner : MonoBehaviour
	{
		[SerializeField]
		private BoxCollider          _bounds;
		[SerializeField]
		private SpawnConfiguration[] _configurations;
		[SerializeField]
		private Transform[]          _ignoreGroups;
		[SerializeField]
		private bool                 _clearOnSpawn;
		[SerializeField]
		private bool                 _clear;
		[SerializeField]
		private bool                 _spawn;

		private void Update()
		{
#if UNITY_EDITOR
			if (_clear == true)
			{
				_clear = false;

				for (int i = transform.childCount - 1; i >= 0; --i)
				{
					GameObject child = transform.GetChild(i).gameObject;
					GameObject.DestroyImmediate(child);
				}
			}

			if (_spawn == false)
				return;

			_spawn = false;

			if (_clearOnSpawn == true)
			{
				for (int i = transform.childCount - 1; i >= 0; --i)
				{
					GameObject child = transform.GetChild(i).gameObject;
					GameObject.DestroyImmediate(child);
				}
			}

			if (_bounds == null)
				return;

			int totalSpawnedItems = 0;

			Vector3 minPosition = _bounds.transform.TransformPoint(_bounds.center.x - _bounds.size.x, 0.0f, _bounds.center.z - _bounds.size.z);
			Vector3 maxPosition = _bounds.transform.TransformPoint(_bounds.center.x + _bounds.size.x, 0.0f, _bounds.center.z + _bounds.size.z);

			for (int j = 0; j < _configurations.Length; ++j)
			{
				SpawnConfiguration configuration = _configurations[j];
				if (configuration.IsEnabled == false)
					continue;

				int   spawnedItems     = 0;
				int   spawnAttempts    = 0;
				int   maxSpawnAttempts = configuration.Count * 100;
				float maxDistanceSqr   = configuration.IgnoreRadius * configuration.IgnoreRadius;

				while (spawnedItems < configuration.Count)
				{
					++spawnAttempts;
					if (spawnAttempts > maxSpawnAttempts)
					{
						Debug.LogError($"Max spawn attempts ({maxSpawnAttempts}) reached. Quitting...");
						break;
					}

					Vector3 point = new Vector3(Random.Range(minPosition.x, maxPosition.x), 0.0f, Random.Range(minPosition.z, maxPosition.z));
					if (Physics.Raycast(point + Vector3.up * 100.0f, Vector3.down, out RaycastHit hitInfo, 200.0f, -1, QueryTriggerInteraction.Ignore) == true)
					{
						if (hitInfo.collider.sharedMaterial == configuration.Material)
						{
							bool canSpawn = true;

							foreach (Transform ignoreGroup in _ignoreGroups)
							{
								for (int childIndex = 0, childCount = ignoreGroup.childCount; childIndex < childCount; ++childIndex)
								{
									Transform child = ignoreGroup.GetChild(childIndex);
									if (Vector3.SqrMagnitude(child.position - hitInfo.point) <= maxDistanceSqr)
									{
										canSpawn = false;
										break;
									}
								}

								if (canSpawn == false)
									break;
							}

							if (canSpawn == false)
								continue;

							for (int borderAngle = 0; borderAngle < 360; borderAngle += 30)
							{
								Vector3 borderPoint = hitInfo.point + Quaternion.Euler(0.0f, borderAngle, 0.0f) * new Vector3(0.0f, 0.0f, configuration.BorderRadius);
								if (Physics.Raycast(borderPoint + Vector3.up * configuration.BorderRadius, Vector3.down, out RaycastHit borderHitInfo, configuration.BorderRadius * 2.0f, -1, QueryTriggerInteraction.Ignore) == true)
								{
									if (borderHitInfo.collider.sharedMaterial != configuration.Material)
									{
										canSpawn = false;
										break;
									}
								}
								else
								{
									canSpawn = false;
									break;
								}
							}

							if (canSpawn == false)
								continue;

							float scale = Random.Range(configuration.MinScale, configuration.MaxScale);

							GameObject instance = UnityEditor.PrefabUtility.InstantiatePrefab(configuration.Prefab) as GameObject;
							instance.transform.SetParent(transform);
							instance.transform.SetPositionAndRotation(hitInfo.point, Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
							instance.transform.localScale = new Vector3(scale, scale, scale);
							instance.name = $"{configuration.Prefab.name} ({totalSpawnedItems})";

							++spawnedItems;
							++totalSpawnedItems;
						}
					}
				}
			}

			Debug.LogWarning($"Spawned {totalSpawnedItems} prefabs.");
#endif
		}

		[Serializable]
		private sealed class SpawnConfiguration
		{
			public bool           IsEnabled;
			public GameObject     Prefab;
			public PhysicMaterial Material;
			public int            Count;
			public float          MinScale     = 1.0f;
			public float          MaxScale     = 1.0f;
			public float          BorderRadius = 0.5f;
			public float          IgnoreRadius = 1.0f;
		}
	}
}
