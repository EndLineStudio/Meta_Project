namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - setting custom ground angle.
	/// </summary>
	public sealed class MaxGroundAngleKCCProcessor : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _maxGroundAngle = 45.0f;
		[SerializeField]
		private int   _relativePriority;

		// KCCProcessor INTERFACE

		// Priority is relative to default ground processor.
		public override float Priority => GroundKCCProcessor.DefaultPriority + _relativePriority;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetInputProperties stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetInputProperties;
		}

		public override void SetInputProperties(KCC kcc, KCCData data)
		{
			data.MaxGroundAngle = _maxGroundAngle;

			// It is OK to suppress other processors of same type with lower priority.
			kcc.SuppressProcessors<MaxGroundAngleKCCProcessor>();

			// The Priority depends on different processor type (GroundKCCProcessor), but is not good idea to suppress it.
			// Other processors can still set various other properties in SetInputProperties stage.

			// The correct sulution here is to have Priority lower than GroundKCCProcessor.DefaultPriority (keeping relative priority below zero).
			// GroundKCCProcessor will set default MaxGroundAngle, then a MaxGroundAngleKCCProcessor instance with highest relative priority overrides MaxGroundAngle and suppress other processors with lower priority.

			// General advice: be careful with mixing base and relative priorities, have strict rules and keep an organized priority table if you want to utilize this system for more complex scenarios.
		}
	}
}
