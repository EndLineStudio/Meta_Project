namespace Example
{
	using System.Collections.Generic;
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Base interface to identify all processors modifying kinematic speed.
	/// This can be treated as a processor category - executing processor with highest priority and suppressing all other processors in same category.
	/// </summary>
	public interface IKinematicSpeedKCCProcessor
	{
	}

	/// <summary>
	/// Example processor - multiplying kinematic speed of the KCC + applying friction based on physics materials.
	/// This processor also implements IMapStatusProvider - providing status text about active slowdown to be shown in UI.
	/// </summary>
	public sealed class KinematicSpeedKCCProcessor : KCCProcessor, IKinematicSpeedKCCProcessor, IMapStatusProvider
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _kinematicSpeedMultiplier = 1.0f;
		[SerializeField]
		private bool _suppressOtherProcessors;
		[SerializeField]
		private int _overridePriority;
		[SerializeField]
		private List<PhysicMaterial> _physicsMaterials;

		private Dictionary<PlayerRef, float>          _activeFrictions = new Dictionary<PlayerRef, float>();
		private Dictionary<PlayerRef, PhysicMaterial> _activeMaterials = new Dictionary<PlayerRef, PhysicMaterial>();

		// PUBLIC METHODS

		// This method is used by all processors modifying kinematic speed to ensure there is a consistent priority calculation.
		// In this case processor priority equals to multiplier. Processor with highest multiplier will be executed first, providing "highest speedup available".
		public static float GetProcessorPriority(float multiplier)
		{
			return multiplier;
		}

		// KCCProcessor INTERFACE

		public override float Priority => GetProcessorPriority(_overridePriority != default ? _overridePriority : _kinematicSpeedMultiplier);

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetKinematicSpeed stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetKinematicSpeed;
		}

		public override void SetKinematicSpeed(KCC kcc, KCCData data)
		{
			float          highestFriction         = default;
			PhysicMaterial highestFrictionMaterial = default;

			if (_kinematicSpeedMultiplier > 0 && _physicsMaterials.Count > 0)
			{
				// Iterate over all collider hits and get highest friction from all physics materials this processor reacts to.
				foreach (KCCHit hit in data.Hits.All)
				{
					PhysicMaterial physicsMaterial = hit.Collider.sharedMaterial;
					if (physicsMaterial != null && _physicsMaterials.Contains(physicsMaterial) == true)
					{
						float friction = data.RealSpeed > 0.001f ? physicsMaterial.dynamicFriction : physicsMaterial.staticFriction;
						if (friction > highestFriction)
						{
							highestFriction         = friction;
							highestFrictionMaterial = physicsMaterial;

							if (highestFriction >= 1.0f)
							{
								highestFriction = 1.0f;
								break;
							}
						}
					}
				}
			}

			// Storing info in non-networked variables is generally not safe here because of prediction/resimulations.
			// In this case it is OK, we use the data only for presentation in UI which doesn't affect gameplay.
			_activeFrictions[kcc.Object.InputAuthority] = highestFriction;
			_activeMaterials[kcc.Object.InputAuthority] = highestFrictionMaterial;

			// Apply multiplier.
			data.KinematicSpeed *= Mathf.Lerp(_kinematicSpeedMultiplier, 0.0f, highestFriction);

			if (_suppressOtherProcessors == true)
			{
				// Suppress all other processors in same category (identified by the interface) with lower priority.
				kcc.SuppressProcessors<IKinematicSpeedKCCProcessor>();
			}
		}

		// IMapStatusProvider INTERFACE

		bool IMapStatusProvider.IsActive(PlayerRef player)
		{
			return _activeFrictions.TryGetValue(player, out float friction) == true && friction > 0.0f;
		}

		string IMapStatusProvider.GetStatus(PlayerRef player)
		{
			if (_activeFrictions.TryGetValue(player, out float friction) == false || friction <= 0.0f)
				return "";
			if (_activeMaterials.TryGetValue(player, out PhysicMaterial material) == false || material == null)
				return "";

			// Status text is simple combination of physics material name and friction converted to percentual slowdown.
			return $"{material.name} - {Mathf.RoundToInt(friction * 100.0f)}% slowdown";
		}
	}
}
