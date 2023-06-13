namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Simple script for testing collisions of Fusion KCC - moves forward or towards a target.
	/// </summary>
	public sealed class TestKCC : TestCC
	{
		// PUBLIC MEMBERS

		public KCC KCC => _kcc;

		// PRIVATE MEMBERS

		private KCC _kcc;

		// TestCC INTERFACE

		public override void ManualFixedUpdate()
		{
			if (HasTarget == true)
			{
				Vector3 direction = (Target.position - _kcc.Data.TargetPosition).OnlyXZ();
				if (direction.sqrMagnitude < 1.0f)
				{
					ClearTarget();
				}

				direction.Normalize();

				_kcc.SetLookRotation(Quaternion.LookRotation(direction));
				_kcc.SetInputDirection(direction);
			}
			else
			{
				_kcc.SetInputDirection(_kcc.Data.TransformDirection);
			}

			_kcc.FixedData.KinematicVelocity = _kcc.FixedData.InputDirection * Speed;

			_kcc.ManualFixedUpdate();

			if (Runner.Stage == SimulationStages.Forward && Object.IsProxy == true)
			{
				_kcc.Interpolate();
			}
		}

		public override void ManualRenderUpdate()
		{
			_kcc.ManualRenderUpdate();
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_kcc = gameObject.GetComponent<KCC>();
			_kcc.Initialize(EKCCDriver.Unity);
			_kcc.SetManualUpdate(true);
		}
	}
}
