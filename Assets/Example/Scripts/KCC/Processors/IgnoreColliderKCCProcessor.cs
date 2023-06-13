namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - adding specific collider to internal KCC ignore list.
	/// In case of blocking collider the KCC will be able to go through and will not interact with it.
	/// Active interaction from the collider will be stopped.
	/// </summary>
	public sealed class IgnoreColliderKCCProcessor : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private Collider _collider;

		// KCCProcessor INTERFACE

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// No KCC stage is used in this processor, we can filter them out to prevent unnecessary method calls.
			return EKCCStages.None;
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			kcc.SetIgnoreCollider(_collider, true);
		}

		public override void OnExit(KCC kcc, KCCData data)
		{
			kcc.SetIgnoreCollider(_collider, false);
		}
	}
}
