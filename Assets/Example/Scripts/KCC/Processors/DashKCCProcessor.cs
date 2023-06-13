namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - moves the player in forward direction over time period.
	/// This processor also reacts on teleport events and recalculates direction if the KCC rotation changes.
	/// This processor has networked state and must be spawned.
	/// </summary>
	public sealed class DashKCCProcessor : NetworkKCCProcessor, IKinematicSpeedKCCProcessor, ITeleportResponder
	{
		// PUBLIC MEMBERS

		public Vector3 Direction => _direction;

		// PRIVATE MEMBERS

		[SerializeField]
		private bool  _allowInputDirection = false;
		[SerializeField]
		private bool  _stopOnReleport = false;
		[SerializeField]
		private bool  _recalculateOnTeleport = true;
		[SerializeField]
		private float _kinematicSpeedMultiplier = 10.0f;
		[SerializeField]
		private float _duration = 0.25f;

		[Networked]
		private Vector3 _direction     { get; set; }
		[Networked]
		private float   _remainingTime { get; set; }

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			// This object is needed on state & input authority only => Object Interest is set to Explicit Players.
			// Input authority should always be a player who owns the dash ability.
			Object.SetPlayerAlwaysInterested(Object.InputAuthority, true);
		}

		// NetworkKCCProcessor INTERFACE

		// Using method shared by all processors modifying kinematic speed to ensure there is a consistent priority calculation.
		public override float Priority => KinematicSpeedKCCProcessor.GetProcessorPriority(_kinematicSpeedMultiplier);

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only following stages are used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetInputProperties | EKCCStages.SetKinematicDirection | EKCCStages.SetKinematicSpeed | EKCCStages.OnStay;
		}

		public override void SetInputProperties(KCC kcc, KCCData data)
		{
			// In case of very fast movement the prediction correction can cause visual glitches.
			// We can disable this feature when the processor is active.
			kcc.SuppressFeature(EKCCFeature.PredictionCorrection);
		}

		public override void SetKinematicDirection(KCC kcc, KCCData data)
		{
			// Override kinematic direction by the value stored on activation.
			data.KinematicDirection = _direction;

			// Skip all other processors.
			kcc.SuppressProcessors<IKCCProcessor>();
		}

		public override void SetKinematicSpeed(KCC kcc, KCCData data)
		{
			data.KinematicSpeed *= _kinematicSpeedMultiplier;

			// Suppress all other processors in same category (identified by the interface) with lower priority.
			kcc.SuppressProcessors<IKinematicSpeedKCCProcessor>();
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			if (kcc.IsInFixedUpdate == false)
				return;

			if (_allowInputDirection == true && data.InputDirection.IsAlmostZero() == false)
			{
				// If the KCC has an input direction, we'll start dashing in this direction.
				_direction = data.InputDirection.normalized;
			}
			else
			{
				// Otherwise we'll start dashing in look direction.
				_direction = Quaternion.Euler(data.LookPitch, data.LookYaw, 0.0f) * Vector3.forward;
			}

			_remainingTime = _duration;
		}

		public override void OnStay(KCC kcc, KCCData data)
		{
			if (kcc.IsInFixedUpdate == false)
				return;

			_remainingTime -= data.DeltaTime;
			if (_remainingTime <= 0.0f)
			{
				// Dash has ended, cleanup.
				kcc.RemoveModifier(this);
			}
		}

		// ITeleportResponder INTERFACE

		void ITeleportResponder.OnTeleport(KCC kcc, KCCData data)
		{
			if (_stopOnReleport == true)
			{
				_remainingTime = 0.0f;
			}

			if (_recalculateOnTeleport == true)
			{
				// Updated dash direction based on new look direction.
				_direction = Quaternion.Euler(data.LookPitch, data.LookYaw, 0.0f) * Vector3.forward;
			}
		}
	}
}
