namespace Example
{
	using System;
	using UnityEngine;
	using UnityEngine.Rendering;
	using UnityEngine.Rendering.Universal;

	/// <summary>
	/// Component to control scene and quality configuration.
	/// </summary>
	public sealed class SceneConfig : MonoBehaviour
	{
		// PRIVATE MEMBERS

		[Header("Post-Processing")]

		[SerializeField]
		private Camera[]      _cameras;
		[SerializeField]
		private Volume[]      _postProcessVolumes;
		[SerializeField]
		private QualityConfig _pcLow;
		[SerializeField]
		private QualityConfig _pcMedium;
		[SerializeField]
		private QualityConfig _pcHigh;
		[SerializeField]
		private QualityConfig _mobileLow;
		[SerializeField]
		private QualityConfig _mobileMedium;
		[SerializeField]
		private QualityConfig _mobileHigh;
		[SerializeField]
		private QualityConfig _vrLow;
		[SerializeField]
		private QualityConfig _vrMedium;
		[SerializeField]
		private QualityConfig _vrHigh;

		// PUBLIC METHODS

		public int GetQualityLevel()
		{
			return QualitySettings.GetQualityLevel() % 3;
		}

		public void SetQualityLevel(int level)
		{
			level %= 3;

			if (ApplicationUtility.IsVREnabled() == true)
			{
				level += 6;
			}
			else if (Application.isMobilePlatform == true && Application.isEditor == false)
			{
				level += 3;
			}

			QualitySettings.SetQualityLevel(level, true);

			QualityConfig config;

			switch (level)
			{
				case 0: { config = _pcLow;        break; }
				case 1: { config = _pcMedium;     break; }
				case 2: { config = _pcHigh;       break; }
				case 3: { config = _mobileLow;    break; }
				case 4: { config = _mobileMedium; break; }
				case 5: { config = _mobileHigh;   break; }
				case 6: { config = _vrLow;        break; }
				case 7: { config = _vrMedium;     break; }
				case 8: { config = _vrHigh;       break; }
				default:
					throw new NotImplementedException($"Level {level}");
			}

			if (_cameras != null)
			{
				foreach (Camera camera in _cameras)
				{
					UniversalAdditionalCameraData cameraData = camera.GetComponent<UniversalAdditionalCameraData>();
					if (cameraData != null)
					{
						cameraData.renderShadows        = config.EnableShadows;
						cameraData.renderPostProcessing = config.EnablePostProcessing;
						cameraData.antialiasing         = config.AntialiasingMode;
					}
				}
			}

			if (_postProcessVolumes != null)
			{
				foreach (Volume volume in _postProcessVolumes)
				{
					volume.profile = config.PostProcessProfile;
				}
			}
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			SetQualityLevel(GetQualityLevel());
		}

		[Serializable]
		private sealed class QualityConfig
		{
			public bool             EnableShadows;
			public bool             EnablePostProcessing;
			public VolumeProfile    PostProcessProfile;
			public AntialiasingMode AntialiasingMode;
		}
	}
}
