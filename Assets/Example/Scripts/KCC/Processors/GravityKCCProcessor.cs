namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - setting custom gravity (absolute value or multiplier).
	/// This processor also implements IMapStatusProvider - providing status text about active gravity effect to be shown in UI.
	/// </summary>
	public sealed class GravityKCCProcessor : KCCProcessor, IMapStatusProvider
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float _gravity = 1.0f;
		[SerializeField]
		private bool  _isMultiplier = true;

		// KCCProcessor INTERFACE

		public override float Priority => default;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetInputProperties stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetInputProperties;
		}

		public override void SetInputProperties(KCC kcc, KCCData data)
		{
			if (_isMultiplier == true)
			{
				data.Gravity *= _gravity;
			}
			else
			{
				data.Gravity = new Vector3(0.0f, _gravity, 0.0f);
			}
		}

		// IMapStatusProvider INTERFACE

		bool IMapStatusProvider.IsActive(PlayerRef player)
		{
			return true;
		}

		string IMapStatusProvider.GetStatus(PlayerRef player)
		{
			if (_isMultiplier == true)
			{
				return $"{name} - {Mathf.RoundToInt(_gravity * 100.0f)}% gravity";
			}
			else
			{
				return $"{name} - Gravity {_gravity}";
			}
		}
	}
}
