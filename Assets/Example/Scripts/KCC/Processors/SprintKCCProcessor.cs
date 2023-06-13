namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - multiplying kinematic speed based on Sprint property.
	/// </summary>
	public sealed class SprintKCCProcessor : KCCProcessor, IKinematicSpeedKCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _kinematicSpeedMultiplier = 2.0f;

		// KCCProcessor INTERFACE

		// Using method shared by all processors modifying kinematic speed to ensure there is a consistent priority calculation.
		public override float Priority => KinematicSpeedKCCProcessor.GetProcessorPriority(_kinematicSpeedMultiplier);

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetKinematicSpeed stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetKinematicSpeed;

			// It would be possible to return EKCCStages.SetKinematicSpeed only if KCCData.Sprint == true. By returning it always we defer the decision to stage itself right before applying speed multiplier.
			// Because the KCCData.Sprint property can be set from SetInputProperties stage (which is called after all GetValidStages()) in other processors, forcing KCC to sprint even if it's not triggered by player input.
		}

		public override void SetKinematicSpeed(KCC kcc, KCCData data)
		{
			// Apply the multiplier only if the Sprint property is set.
			if (data.Sprint == true)
			{
				data.KinematicSpeed *= _kinematicSpeedMultiplier;

				// Suppress all other processors in same category (identified by the interface) with lower priority.
				kcc.SuppressProcessors<IKinematicSpeedKCCProcessor>();
			}
		}
	}
}
