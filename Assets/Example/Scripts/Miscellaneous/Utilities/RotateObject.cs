namespace Example
{
	using UnityEngine;

	public sealed class RotateObject : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _speed;

		private void Update()
		{
			transform.Rotate(_speed * Time.deltaTime);
		}
	}
}
