namespace Example
{
	using Fusion;
	using Fusion.KCC;
	using UnityEngine;

	/// <summary>
	/// Example processor - applying external force to attract the KCC to a specific target.
	/// </summary>
	public sealed class Attractor : KCCProcessor, IMapStatusProvider
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private SphereCollider _target;
		[SerializeField]
		private AnimationCurve _curve;
		[SerializeField]
		private float _force;

		// KCCProcessor INTERFACE

		public override float Priority => default;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only OnStay is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.OnStay;
		}

		public override void OnStay(KCC kcc, KCCData data)
		{
			// Calculate direction and power of the attraction.

			Vector3 direction = _target.transform.TransformPoint(_target.center) - data.BasePosition;
			float   distance  = Vector3.Magnitude(direction);
			float   power     = 0.0f;

			if (distance > 0.001f)
			{
				Vector3 lossyScale   = _target.transform.lossyScale;
				float   scaledRadius = _target.radius * Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);

				direction /= distance;
				power = _force * _curve.Evaluate(Mathf.Clamp01(distance / scaledRadius));
			}

			// Apply calculated force.
			kcc.AddExternalForce(direction * power);
		}

		// IMapStatusProvider INTERFACE

		bool IMapStatusProvider.IsActive(PlayerRef player)
		{
			return true;
		}

		string IMapStatusProvider.GetStatus(PlayerRef player)
		{
			return $"Attracting to {name}";
		}
	}
}
