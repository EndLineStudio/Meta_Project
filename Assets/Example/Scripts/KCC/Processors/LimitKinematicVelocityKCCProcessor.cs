namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - clearing kinematic velocity if the real speed is greater than threshold.
	/// This processor removes itself from owner KCC when it is done (real speed is below threshold).
	/// </summary>
	public sealed class LimitKinematicVelocityKCCProcessor : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _clearKinematicVelocityIfAboveSpeed;

		// KCCProcessor INTERFACE

		// This processor needs highest priority to execute first and suppress all other processors
		public override float Priority => float.MaxValue;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetKinematicSpeed and OnStay stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetKinematicVelocity | EKCCStages.OnStay;
		}

		public override void SetKinematicVelocity(KCC kcc, KCCData data)
		{
			// If the velocity is successfully cleared (real speed was greater than threshold), all processors with lower priority are suppressed
			if (TryClearVelocity(data) == true)
			{
				// IKCCProcessor is the base type for all processors. Do not confuse with KCCProcessor, which implements it.
				kcc.SuppressProcessors<IKCCProcessor>();
			}
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			TryClearVelocity(data);
		}

		public override void OnStay(KCC kcc, KCCData data)
		{
			if (_clearKinematicVelocityIfAboveSpeed > 0.0f && data.RealSpeed >= _clearKinematicVelocityIfAboveSpeed)
				return;

			// Overall speed is below threshold, we can remove self from the KCC.
			kcc.RemoveModifier(this);
		}

		// PRIVATE METHODS

		private bool TryClearVelocity(KCCData data)
		{
			if (_clearKinematicVelocityIfAboveSpeed > 0.0f && data.RealSpeed >= _clearKinematicVelocityIfAboveSpeed)
			{
				data.KinematicVelocity = Vector3.zero;
				return true;
			}

			return false;
		}
	}
}
