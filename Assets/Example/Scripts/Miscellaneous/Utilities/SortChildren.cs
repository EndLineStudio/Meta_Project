namespace Example
{
	using UnityEngine;

	[ExecuteAlways]
	public sealed class SortChildren : MonoBehaviour
	{
		[SerializeField]
		private float _distance = 2.0f;
		[SerializeField]
		private int   _itemsPerRow = 10;

		private float _currentDistance;
		private int   _currentItemsPerRow;

		private void Update()
		{
			if (_currentDistance == _distance && _currentItemsPerRow == _itemsPerRow)
				return;

			_currentDistance    = _distance;
			_currentItemsPerRow = _itemsPerRow;

			int row    = 0;
			int column = 0;

			foreach (Transform child in transform)
			{
				Vector3 localPosition = Vector3.zero;

				localPosition.x = _distance * row;
				localPosition.z = _distance * column;

				child.localPosition = localPosition;

				++row;

				if (row >= _itemsPerRow)
				{
					row = 0;
					++column;
				}
			}
		}
	}
}
