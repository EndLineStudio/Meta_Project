namespace Example
{
	using UnityEngine;

	[ExecuteAlways]
	public sealed class PrefabReplacer : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] _prefabs;
		[SerializeField]
		private bool         _replace;

		private void Update()
		{
#if UNITY_EDITOR
			if (_replace == false)
				return;

			_replace = false;

			string[] prefabNames = new string[_prefabs.Length];
			for (int j = 0; j < _prefabs.Length; ++j)
			{
				prefabNames[j] = _prefabs[j].name + " (";
			}

			for (int i = transform.childCount - 1; i >= 0; --i)
			{
				GameObject child = transform.GetChild(i).gameObject;

				for (int j = 0; j < _prefabs.Length; ++j)
				{
					if (child.name.StartsWith(prefabNames[j]) == true)
					{
						GameObject instance = UnityEditor.PrefabUtility.InstantiatePrefab(_prefabs[j]) as GameObject;
						instance.transform.SetParent(transform);
						instance.transform.SetPositionAndRotation(child.transform.position, child.transform.rotation);
						instance.name = child.name;
						GameObject.DestroyImmediate(child);
						break;
					}
				}
			}
#endif
		}
	}
}
