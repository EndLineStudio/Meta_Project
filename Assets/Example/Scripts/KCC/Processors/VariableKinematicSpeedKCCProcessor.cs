namespace Example
{
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - multiplying kinematic speed based on a variable multiplier.
	/// </summary>
	public sealed class VariableKinematicSpeedKCCProcessor : NetworkKCCProcessor, IKinematicSpeedKCCProcessor
	{
		// PUBLIC MEMBERS

		[Networked]
		public float KinematicSpeedMultiplier { get; set; }

		// NetworkKCCProcessor INTERFACE

		// Using method shared by all processors modifying kinematic speed to ensure there is a consistent priority calculation.
		public override float Priority => KinematicSpeedKCCProcessor.GetProcessorPriority(KinematicSpeedMultiplier);

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetKinematicSpeed stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetKinematicSpeed;
		}

		public override void FixedUpdateNetwork()
		{
			// Updating speed multiplier from default Fusion method.
			// The processor has to be spawned by Fusion, otherwise this method will not be executed and you'll get exceptions when accessing [Networked] properties.
			// Execution of all IKCCProcessor methods is supported on 1) Prefabs, 2) Instances spawned with GameObject.Instantiate(), 3) Instances spawned with Runner.Spawn()

			KinematicSpeedMultiplier += Runner.DeltaTime;
			if (KinematicSpeedMultiplier > 8.0f)
			{
				KinematicSpeedMultiplier = 1.0f;
			}
		}

		public override void SetKinematicSpeed(KCC kcc, KCCData data)
		{
			data.KinematicSpeed *= KinematicSpeedMultiplier;

			// Suppress all other processors in same category (identified by the interface) with lower priority.
			kcc.SuppressProcessors<IKinematicSpeedKCCProcessor>();
		}
	}
}
