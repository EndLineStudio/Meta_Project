namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Simple script for testing collisions of Unity CC - moves forward or towards a target.
	/// </summary>
	public sealed class TestUCC : TestCC
	{
		// PUBLIC MEMBERS

		public CharacterController UCC => _ucc;

		// PRIVATE MEMBERS

		private Transform           _transform;
		private CharacterController _ucc;

		// TestCC INTERFACE

		public override void ManualFixedUpdate()
		{
			if (HasTarget == true)
			{
				Vector3 direction = (Target.position - transform.position).OnlyXZ();
				if (direction.sqrMagnitude < 1.0f)
				{
					ClearTarget();
				}

				direction.Normalize();

				transform.rotation = Quaternion.LookRotation(direction);
				_ucc.SimpleMove(direction * Speed);
			}
			else
			{
				_ucc.SimpleMove(_transform.forward.OnlyXZ() * Speed);
			}
		}

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			if (Object.IsSceneObject == false)
			{
				_ucc.Move(_transform.position);
			}
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_transform = transform;
			_ucc       = GetComponent<CharacterController>();
		}
	}
}
