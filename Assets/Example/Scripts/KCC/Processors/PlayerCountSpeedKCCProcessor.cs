namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - multiplying kinematic speed based on player interacting with the processor.
	/// </summary>
	public sealed class PlayerCountSpeedKCCProcessor : NetworkKCCProcessor, IKinematicSpeedKCCProcessor
	{
		// PUBLIC MEMBERS

		// PlayerCount needs to be networked to support rollback
		[Networked][HideInInspector]
		public int PlayerCount { get; set; }

		// NetworkKCCProcessor INTERFACE

		// Using method shared by all processors modifying kinematic speed to ensure there is a consistent priority calculation.
		// In this case speed multiplier is based on player count interacting with the processor, which is then used for priority calculation.
		public override float Priority => KinematicSpeedKCCProcessor.GetProcessorPriority(PlayerCount * 2);

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetKinematicSpeed stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetKinematicSpeed;
		}

		public override void SetKinematicSpeed(KCC kcc, KCCData data)
		{
			data.KinematicSpeed *= PlayerCount * 2;

			// Suppress all other processors in same category (identified by the interface) with lower priority.
			kcc.SuppressProcessors<IKinematicSpeedKCCProcessor>();
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			// Player count is updated only in fixed updates to simplify things and not mess with render updates.
			// This is enough for most cases where you don't need instant feedback for render.

			if (kcc.IsInFixedUpdate == true)
			{
				PlayerCount = Mathf.Min(PlayerCount + 1, 8);
			}
		}

		public override void OnExit(KCC kcc, KCCData data)
		{
			if (kcc.IsInFixedUpdate == true)
			{
				PlayerCount = Mathf.Max(0, PlayerCount - 1);
			}
		}
	}
}
