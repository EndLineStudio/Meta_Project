namespace Example
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using UnityEngine.InputSystem;

	public sealed class InputSmoothingTest : MonoBehaviour
	{
		[SerializeField]
		private int     _inputBufferSize = 128;
		[SerializeField]
		private float[] _responsivities;

		private List<InputSmoothing> _inputSmoothings = new List<InputSmoothing>();
		private StatsRecorder        _statsRecorder;

		private void Awake()
		{
			System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			string fileID        = $"{System.DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
			string statsFileName = $"InputSmoothing_{fileID}.log";

			List<string> headers = new List<string>();

			headers.Add("Frame");
			headers.Add("Delta Time [ms]");
			headers.Add("Frame Rate");

			for (int i = 0; i < _responsivities.Length; ++i)
			{
				int responsivityMs = Mathf.RoundToInt(_responsivities[i] * 1000.0f + 0.1f);

				headers.Add($"Mouse Delta {responsivityMs}ms");
				headers.Add($"Accumulated Delta {responsivityMs}ms");
				headers.Add($"Frames {responsivityMs}ms");

				_inputSmoothings.Add(new InputSmoothing(_inputBufferSize, _responsivities[i]));
			}

			_statsRecorder = new StatsRecorder();
			_statsRecorder.Initialize(statsFileName, default, fileID, headers.ToArray());
		}

		private void Update()
		{
			if (Time.frameCount < 10)
				return;

			float   deltaTime  = Time.unscaledDeltaTime;
			Vector2 mouseDelta = new Vector2(Mouse.current.delta.ReadValue().x * 0.05f, 0.0f);

			_statsRecorder.Add($"{Time.frameCount}");
			_statsRecorder.Add($"{(Time.unscaledDeltaTime * 1000.0f):F4}");
			_statsRecorder.Add($"{(int)(1.0 / Time.unscaledDeltaTime)}");

			for (int i = 0; i < _inputSmoothings.Count; ++i)
			{
				InputSmoothing inputSmoothing = _inputSmoothings[i];
				inputSmoothing.Process(mouseDelta);

				_statsRecorder.Add($"{inputSmoothing.SmoothedMouseDelta.x:F4}");
				_statsRecorder.Add($"{inputSmoothing.AccumulatedMouseDelta.x:F4}");
				_statsRecorder.Add($"{inputSmoothing.SmoothingFrameCount}");
			}

			_statsRecorder.Write();
		}

		private void OnDestroy()
		{
			_statsRecorder.Flush();
		}

		private sealed class InputSmoothing
		{
			public Vector2 SmoothedMouseDelta    => _smoothedMouseDelta;
			public Vector2 AccumulatedMouseDelta => _accumulatedMouseDelta;
			public int     SmoothingFrameCount   => _smoothingFrameCount;

			private float         _responsivity;
			private Vector2       _smoothedMouseDelta;
			private Vector2       _accumulatedMouseDelta;
			private int           _smoothingFrameCount;
			private FrameRecord[] _frameRecords;

			public InputSmoothing(int samples, float responsivity)
			{
				_responsivity = responsivity;
				_frameRecords = new FrameRecord[samples];
			}

			public void Process(Vector2 mouseDelta)
			{
				Process(mouseDelta, out _smoothedMouseDelta, out _smoothingFrameCount);

				_accumulatedMouseDelta += _smoothedMouseDelta;
			}

			private void Process(Vector2 mouseDelta, out Vector2 smoothedMouseDelta, out int smoothingFrames)
			{
				smoothingFrames = 1;

				if (_responsivity <= 0.0f)
				{
					smoothedMouseDelta = mouseDelta;
					return;
				}

				FrameRecord   frameRecord  = new FrameRecord(Time.unscaledDeltaTime, mouseDelta);
				FrameRecord[] frameRecords = _frameRecords;

				Array.Copy(frameRecords, 0, frameRecords, 1, frameRecords.Length - 1);

				frameRecords[0] = frameRecord;

				float   accumulatedDeltaTime         = default;
				Vector2 accumulatedLookRotationDelta = default;

				for (int i = 0; i < frameRecords.Length; ++i)
				{
					frameRecord = frameRecords[i];

					accumulatedDeltaTime         += frameRecord.DeltaTime;
					accumulatedLookRotationDelta += frameRecord.DeltaTime * frameRecord.LookRotationDelta;

					if (accumulatedDeltaTime > _responsivity)
					{
						float overshootDeltaTime = accumulatedDeltaTime - _responsivity;

						accumulatedDeltaTime         -= overshootDeltaTime;
						accumulatedLookRotationDelta -= overshootDeltaTime * frameRecord.LookRotationDelta;

						break;
					}

					++smoothingFrames;
				}

				// Normalize acucmulated look rotation delta and calculate size for current frame.
				smoothedMouseDelta = accumulatedLookRotationDelta / accumulatedDeltaTime;
			}
		}

		private struct FrameRecord
		{
			public float   DeltaTime;
			public Vector2 LookRotationDelta;

			public FrameRecord(float deltaTime, Vector2 lookRotationDelta)
			{
				DeltaTime         = deltaTime;
				LookRotationDelta = lookRotationDelta;
			}
		}
	}
}
