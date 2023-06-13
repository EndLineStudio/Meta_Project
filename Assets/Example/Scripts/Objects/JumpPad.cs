namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - applying dynamic impulse to get KCC to a specific destination.
	/// </summary>
	public sealed class JumpPad : KCCProcessor, IAirKCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private Transform _destination;

		// KCCProcessor INTERFACE

		public override float Priority => float.MaxValue;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// SetDynamicVelocity is used to apply gravity only
			// SetKinematicVelocity is used to suppress all kinematic movement
			// ProcessPhysicsQuery is used to check landing and removing self from KCC modifiers
			return EKCCStages.SetDynamicVelocity | EKCCStages.SetKinematicVelocity | EKCCStages.ProcessPhysicsQuery;
		}

		public override void SetDynamicVelocity(KCC kcc, KCCData data)
		{
			// Applying gravity. Notice KCCData.Gravity is already modified by AirKCCProcessor.SetInputProperties()
			data.DynamicVelocity += data.Gravity * data.DeltaTime;

			// External forces and friction are ignored in this processor.

			// Suppress other processors
			kcc.SuppressProcessors<IKCCProcessor>();
		}

		public override void SetKinematicVelocity(KCC kcc, KCCData data)
		{
			// Suppress kinematic movement completely
			data.KinematicVelocity = default;

			// Suppress other processors
			kcc.SuppressProcessors<IKCCProcessor>();
		}

		public override void ProcessPhysicsQuery(KCC kcc, KCCData data)
		{
			if (data.IsGrounded == true && data.WasGrounded == true)
			{
				// Game over, we can remove the processor from KCC modifiers
				kcc.RemoveModifier(this);
			}
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			// Ignore if the KCC is not grounded
			if (data.IsGrounded == false)
				return;
			if (data.TargetPosition.y >= _destination.position.y)
				return;

			// Clear kinematic and dynamic velocity entirely
			kcc.SetKinematicVelocity(Vector3.zero);
			kcc.SetDynamicVelocity(Vector3.zero);

			// Explicitly set current position, this kills any remaining movement (CCD might be active)
			kcc.SetPosition(data.TargetPosition);

			Vector3 gravity = data.Gravity;

			if (data.WasGrounded == true)
			{
				// In this case KCCData.Gravity is driven by GroundKCCProcessor (unmodified) but will be affected by AirKCCProcessor.GravityMultiplier next frame
				AirKCCProcessor airProcessor = kcc.GetProcessor<AirKCCProcessor>();
				if (airProcessor != null)
				{
					gravity *= airProcessor.GravityMultiplier;
				}
			}

			// Force un-ground KCC
			data.IsGrounded = false;

			// Calculate how long it takes to reach apex
			Vector3 offset   = _destination.position - data.TargetPosition;
			float   apexTime = Mathf.Sqrt(-2.0f * offset.y / gravity.y) + data.UnscaledDeltaTime * 0.5f;

			// Calculate initial velocity
			Vector3 velocity = offset.OnlyXZ() / apexTime - gravity.OnlyY() * apexTime;

			kcc.SetDynamicVelocity(velocity);

			// Adding self as KCC modifier to override dynamic velocity behavior while not grounded
			// Be careful with adding self as modifier if the current invoke is coming from collision
			// Following call will immediately invoke another OnEnter()
			kcc.AddModifier(this);
		}
	}
}
