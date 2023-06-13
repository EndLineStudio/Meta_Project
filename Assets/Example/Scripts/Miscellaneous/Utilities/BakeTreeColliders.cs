namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	public sealed class BakeTreeColliders : MonoBehaviour
	{
		// PUBLIC MEMBERS

		public TerrainCollider TerrainCollider;
		public bool            BakeAtRuntime;

		// PUBLIC METHODS

		public void Bake()
		{
			Transform       transform      = this.transform;
			TerrainData     terrainData    = TerrainCollider.terrainData;
			TreeInstance[]  treeInstances  = terrainData.treeInstances;
			TreePrototype[] treePrototypes = terrainData.treePrototypes;
			TreeCollider[]  treeColliders  = new TreeCollider[treePrototypes.Length];

			Vector3 terrainSize     = terrainData.size;
			Vector3 terrainPosition = TerrainCollider.transform.position;

			for (int i = 0, count = treePrototypes.Length; i < count; ++i)
			{
				TreePrototype treePrototype = treePrototypes[i];
				if (treePrototype.prefab != null)
				{
					Collider treeCollider = treePrototype.prefab.GetComponentInChildren<Collider>();
					if (treeCollider != null)
					{
						treeColliders[i] = new TreeCollider(treePrototype.prefab, treeCollider);
					}
				}
			}

			Vector3 position;

			for (int i = 0, count = treeInstances.Length; i < count; ++i)
			{
				TreeInstance treeInstance = treeInstances[i];
				TreeCollider treeCollider = treeColliders[treeInstance.prototypeIndex];

				if (treeCollider == null)
					continue;

				position = treeInstance.position;
				position.x *= terrainSize.x;
				position.y *= terrainSize.y;
				position.z *= terrainSize.z;

				position += terrainPosition;
				position += Quaternion.Euler(0.0f, treeInstance.rotation * Mathf.Rad2Deg, 0.0f) * treeCollider.Offset;

				GameObject colliderGO = new GameObject($"TreeCollider({i})-{treeCollider.Prefab.name}");
				colliderGO.layer = treeCollider.Layer;
				colliderGO.tag = treeCollider.Tag;
				colliderGO.transform.SetParent(transform, false);
				colliderGO.transform.position = position;

				float widthScale = treeInstance.widthScale;

				if (treeCollider.Collider is BoxCollider treeBoxCollider)
				{
					BoxCollider boxCollider = colliderGO.AddComponent<BoxCollider>();
					boxCollider.material = treeBoxCollider.material;
					boxCollider.center   = treeBoxCollider.center;
					boxCollider.size     = treeBoxCollider.size.OnlyXZ() * widthScale + treeBoxCollider.size.OnlyY();
					boxCollider.center   = Quaternion.Euler(0.0f, treeInstance.rotation * Mathf.Rad2Deg, 0.0f) * boxCollider.center;
				}
				else if (treeCollider.Collider is CapsuleCollider treeCapsuleCollider)
				{
					CapsuleCollider capsuleCollider = colliderGO.AddComponent<CapsuleCollider>();
					capsuleCollider.direction = treeCapsuleCollider.direction;
					capsuleCollider.material  = treeCapsuleCollider.material;
					capsuleCollider.center    = treeCapsuleCollider.center;
					capsuleCollider.height    = treeCapsuleCollider.height;
					capsuleCollider.radius    = treeCapsuleCollider.radius * widthScale;
					capsuleCollider.center    = Quaternion.Euler(0.0f, treeInstance.rotation * Mathf.Rad2Deg, 0.0f) * capsuleCollider.center;
				}
				else if (treeCollider.Collider is MeshCollider treeMeshCollider)
				{
					MeshCollider meshCollider = colliderGO.AddComponent<MeshCollider>();
					meshCollider.material   = treeMeshCollider.material;
					meshCollider.sharedMesh = treeMeshCollider.sharedMesh;
					meshCollider.convex     = treeMeshCollider.convex;
				}
				else if (treeCollider.Collider is SphereCollider treeSphereCollider)
				{
					SphereCollider sphereCollider = colliderGO.AddComponent<SphereCollider>();
					sphereCollider.material = treeSphereCollider.material;
					sphereCollider.center   = treeSphereCollider.center;
					sphereCollider.radius   = treeSphereCollider.radius * widthScale;
					sphereCollider.center   = Quaternion.Euler(0.0f, treeInstance.rotation * Mathf.Rad2Deg, 0.0f) * sphereCollider.center;
				}
			}
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			if (BakeAtRuntime == true)
			{
				Bake();
			}
		}

		// DATA STRUCTURES

		private sealed class TreeCollider
		{
			public GameObject Prefab;
			public Collider   Collider;
			public Transform  Transform;
			public GameObject GameObject;
			public Vector3    Offset;
			public int        Layer;
			public string     Tag;

			public TreeCollider(GameObject prefab, Collider collider)
			{
				Prefab     = prefab;
				Collider   = collider;
				Transform  = collider.transform;
				GameObject = collider.gameObject;
				Offset     = Transform.position - prefab.transform.position;
				Layer      = GameObject.layer;
				Tag        = GameObject.tag;
			}
		}
	}
}
