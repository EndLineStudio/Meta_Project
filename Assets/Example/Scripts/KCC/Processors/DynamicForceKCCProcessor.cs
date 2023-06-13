namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - applying dynamic force every frame to all KCCs interacting with the processor.
	/// Interaction can be provided manually (via KCC.AddModifier() call) or collision-based
	/// </summary>
	public sealed class DynamicForceKCCProcessor : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private Vector3 _force;
		[SerializeField]
		private float   _maxDistance;

		// KCCProcessor INTERFACE

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only OnStay stage is used, we can filter out other to prevent unnecessary method calls.
			return EKCCStages.OnStay;
		}

		public override void OnStay(KCC kcc, KCCData data)
		{
			if (_force.IsZero() == true)
				return;

			Vector3 rotatedForce = transform.rotation * _force;

			if (_maxDistance > 0.0f)
			{
				// Magnitude of the force depends on distance between KCC and processor.

				rotatedForce *= 1.0f - Mathf.Clamp(Vector3.Distance(transform.position, data.TargetPosition), 0.0f, _maxDistance) / _maxDistance;
			}

			// Force is applied every KCC update (both fixed and render)

			kcc.AddExternalForce(rotatedForce);
		}
	}
}
