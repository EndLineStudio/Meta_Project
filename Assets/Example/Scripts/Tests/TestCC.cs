namespace Example
{
	using UnityEngine;
	using Fusion;

	/// <summary>
	/// Base script for testing CCs.
	/// </summary>
	public abstract class TestCC : NetworkBehaviour
	{
		// PUBLIC MEMBERS

		public float     Speed     => _speed;
		public Transform Target    => _target;
		public bool      HasTarget => _hasTarget;

		// PRIVATE MEMBERS

		private float     _speed = 2.0f;
		private Transform _target;
		private bool      _hasTarget;
		private bool      _hasManualUpdate;

		// PUBLIC METHODS

		public void SetManualUpdate(bool hasManualUpdate)
		{
			_hasManualUpdate = hasManualUpdate;
		}

		public void SetSpeed(float speed)
		{
			_speed = speed;
		}

		public void SetTarget(Transform target)
		{
			_target    = target;
			_hasTarget = target != null;
		}

		public void ClearTarget()
		{
			_target    = null;
			_hasTarget = false;
		}

		// NetworkBehaviour INTERFACE

		public override void FixedUpdateNetwork()
		{
			if (_hasManualUpdate == false)
			{
				ManualFixedUpdate();
			}
		}

		public override void Render()
		{
			if (_hasManualUpdate == false)
			{
				ManualRenderUpdate();
			}
		}

		// TestCC INTERFACE

		public virtual void ManualFixedUpdate()  {}
		public virtual void ManualRenderUpdate() {}
	}
}
