namespace Example
{
	using UnityEngine;

	public sealed class FrameRateUpdater : MonoBehaviour
	{
		// PUBLIC MEMBERS

		public int TargetFrameRateStandalone = 288;
		public int TargetFrameRateMobile     = 60;

		public int SmoothFrameRate => _smoothFrameRate;

		// PRIVATE MEMBERS

		private float[] _deltaTimes = new float[100];
		private int     _deltaTimeIndex;
		private int     _smoothFrameRate;

		// MonoBehaviour INTERFACE

		private void Update()
		{
			if (Application.isMobilePlatform == true && Application.isEditor == false)
			{
				Application.targetFrameRate = TargetFrameRateMobile;
			}
			else
			{
				Application.targetFrameRate = TargetFrameRateStandalone;
			}

			_deltaTimeIndex = (_deltaTimeIndex + 1) % _deltaTimes.Length;
			_deltaTimes[_deltaTimeIndex] = Time.unscaledDeltaTime;

			float totalDeltaTime = 0.0f;
			for (int i = 0; i < _deltaTimes.Length; ++i)
			{
				totalDeltaTime += _deltaTimes[i];
			}
			totalDeltaTime /= _deltaTimes.Length;

			_smoothFrameRate = totalDeltaTime > 0.000001f ? Mathf.RoundToInt(1.0f / totalDeltaTime) : 0;
		}
	}
}
