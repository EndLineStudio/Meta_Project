namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - applying dynamic impulse to a KCC when it starts interacting with the processor.
	/// Interaction can be provided manually (via KCC.AddModifier() call) or collision-based
	/// </summary>
	public sealed class DynamicImpulseKCCProcessor : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private Vector3 _impulse;
		[SerializeField]
		private LimitKinematicVelocityKCCProcessor _limitKinematicVelocityProcessor;

		// KCCProcessor INTERFACE

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// No KCC stage is used in this processor, we can filter them out to prevent unnecessary method calls.
			return EKCCStages.None;
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			if (_impulse.IsZero() == true)
				return;

			Vector3 rotatedImpulse = transform.rotation * _impulse;

			// Clear kinematic velocity entirely
			kcc.SetKinematicVelocity(Vector3.zero);

			// Clear dynamic velocity proportionaly to impulse direction
			kcc.SetDynamicVelocity(data.DynamicVelocity - Vector3.Scale(data.DynamicVelocity, rotatedImpulse.normalized));

			// Explicitly set current position, this kills any remaining movement (CCD might be active)
			kcc.SetPosition(data.TargetPosition);

			// Add impulse
			kcc.AddExternalImpulse(rotatedImpulse);

			// Add special processor which prevents kinematic movement
			kcc.AddModifier(_limitKinematicVelocityProcessor);
		}
	}
}
