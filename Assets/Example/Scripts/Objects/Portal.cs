namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - teleporting between portals.
	/// </summary>
	public sealed class Portal : KCCProcessor, IMapStatusProvider
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private Portal _target;

		// KCCProcessor INTERFACE

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// No KCC stage is used in this processor, we can filter them out to prevent unnecessary method calls.
			return EKCCStages.None;
		}

		public override void OnEnter(KCC kcc, KCCData data)
		{
			// The portal is virtually split into 2 parts. The KCC is teleported when root position enters back part - controlled by CanStartInteraction().
			// Also there's a small tolerance, otherwise the KCC teleports back and forth.

			// Teleport only in fixed update to not introduce glitches caused by incorrect render extrapolation.
			if (kcc.IsInFixedUpdate == false)
				return;

			// Teleporting from back side of first portal to front side of target portal.
			// Local position offset to target portal equals to local position offset to source portal (this) rotated by 180 degrees.
			Vector3 positionOffset = Quaternion.Euler(0.0f, 180.0f, 0.0f) * transform.InverseTransformPoint(data.TargetPosition);
			kcc.SetPosition(_target.transform.TransformPoint(positionOffset));

			// We also apply rotation offset between both portals.
			Vector3 rotationOffset = Quaternion.FromToRotation(transform.forward, -_target.transform.forward).eulerAngles;
			kcc.AddLookRotation(0.0f, rotationOffset.y);

			// As the last step, we have to rotate all related properties to continue movement in correct direction.
			Quaternion rotation = Quaternion.Euler(0.0f, rotationOffset.y, 0.0f);
			data.DynamicVelocity    = rotation * data.DynamicVelocity;
			data.KinematicDirection = rotation * data.KinematicDirection;
			data.KinematicTangent   = rotation * data.KinematicTangent;
			data.KinematicVelocity  = rotation * data.KinematicVelocity;

			// Notify all responders
			foreach (ITeleportResponder responder in kcc.GetProcessors<ITeleportResponder>(true))
			{
				responder.OnTeleport(kcc, data);
			}
		}

		public override bool CanStartInteraction(KCC kcc, KCCData data)
		{
			if (_target == null)
				return false;

			// We defer interaction start until the KCC crosses center of the object + minor tolerance.
			// This method is called for each CCD step in which KCC overlaps portal collider until true is returned.
			// This approach will work for very fast entities (a special KCC bullet for example or when using Dash ability).
			// OnEnter() is called once this method returns true.

			// Similar functionality can be achieved by not overriding this method and implementing same position check + teleport in ProcessPhysicsQuery().
			// Using OnStay() is not recommended as it is not executed after each CCD step and fast entities can pass through.

			Vector3 localPosition = transform.InverseTransformPoint(data.TargetPosition);
			return localPosition.z > 0.1f;
		}

		// IMapStatusProvider INTERFACE

		bool IMapStatusProvider.IsActive(PlayerRef player)
		{
			return true;
		}

		string IMapStatusProvider.GetStatus(PlayerRef player)
		{
			return $"{name} -> {_target.name}";
		}
	}
}
