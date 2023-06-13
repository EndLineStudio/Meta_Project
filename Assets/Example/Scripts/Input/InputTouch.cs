namespace Example
{
	using UnityEngine;
	using UnityEngine.InputSystem.Controls;
	using Fusion.KCC;

	using TouchPhase = UnityEngine.InputSystem.TouchPhase;

	public sealed class InputTouch
	{
		// CONSTANTS

		private const int HISTORY_SIZE = 10;

		// PUBLIC MEMBERS

		public int              ID       => _id;
		public int              TouchID  => _touchID;
		public InputTouchDelta  Delta    => _delta;
		public InputTouchRecord Start    => _start;
		public InputTouchRecord Current  => _current;
		public InputTouchRecord Previous => _previous;
		public InputTouchRecord End      => _end;

		public bool IsActive             => _current.State == ETouchState.Started || _current.State == ETouchState.Stationary || _current.State == ETouchState.Moving || _current.State == ETouchState.Resumed;
		public bool IsPaused             => _current.State == ETouchState.Paused;
		public bool IsFinished           => _current.State == ETouchState.Finished;
		public bool WasPausedThisFrame   => _current.State == ETouchState.Paused   && _current.Frame == Time.frameCount;
		public bool WasResumedThisFrame  => _current.State == ETouchState.Resumed  && _current.Frame == Time.frameCount;
		public bool WasFinishedThisFrame => _current.State == ETouchState.Finished && _current.Frame == Time.frameCount;

		// PRIVATE MEMBERS

		private int                _id;
		private int                _touchID;
		private InputTouchDelta    _delta;
		private InputTouchRecord   _start;
		private InputTouchRecord   _current;
		private InputTouchRecord   _previous;
		private InputTouchRecord   _end;
		private InputTouchRecord[] _history;

		// CONSTRUCTORS

		public InputTouch(int id, TouchControl touchControl)
		{
			_id      = id;
			_touchID = touchControl.touchId.ReadValue();

			_current.ID       = 0;
			_current.State    = ETouchState.Started;
			_current.Frame    = Time.frameCount;
			_current.Time     = Time.unscaledTime;
			_current.Position = touchControl.position.ReadValue();

			_start = _current;

			_history = new InputTouchRecord[HISTORY_SIZE];
			_history[0] = _current;
		}

		private InputTouch()
		{
		}

		// PUBLIC METHODS

		public InputTouchRecord GetRecord(int offset = 0)
		{
			if (offset < 0 || offset >= HISTORY_SIZE)
				return default;

			return _history[offset];
		}

		public InputTouchDelta GetDelta(int count = 0, int offset = 0)
		{
			if (offset < 0 || count < 0 || offset >= HISTORY_SIZE || (offset + count) >= HISTORY_SIZE)
				return default;

			InputTouchRecord from = count > 0 ? _history[offset + count] : _start;
			InputTouchRecord to   = _history[offset];

			if (from.IsValid == false || to.IsValid == false)
				return default;

			InputTouchDelta delta = default;
			delta.IsValid  = true;
			delta.Count    = to.ID       - from.ID;
			delta.Frames   = to.Frame    - from.Frame;
			delta.Time     = to.Time     - from.Time;
			delta.Position = to.Position - from.Position;

			return delta;
		}

		public void Update(TouchControl touchControl)
		{
			if (_current.State == ETouchState.Finished)
				return;
			if (_current.Frame == Time.frameCount)
				return;

			TouchPhase phase = touchControl.phase.ReadValue();
			if (_current.State == ETouchState.Paused && phase != TouchPhase.None && phase != TouchPhase.Ended && phase != TouchPhase.Canceled)
				return;

			for (int i = _history.Length - 1; i > 0; --i)
			{
				_history[i] = _history[i - 1];
			}
			_history[0] = default;

			_previous = _current;
			_current  = default;
			_delta    = default;

			_current.ID       = _previous.ID + 1;
			_current.Frame    = Time.frameCount;
			_current.Time     = Time.unscaledTime;
			_current.Position = touchControl.position.ReadValue();

			_delta.IsValid  = true;
			_delta.Count    = _current.ID       - _previous.ID;
			_delta.Frames   = _current.Frame    - _previous.Frame;
			_delta.Time     = _current.Time     - _previous.Time;
			_delta.Position = _current.Position - _previous.Position;

			if (_delta.Position.IsZero() == true)
			{
				_current.State = ETouchState.Stationary;
			}
			else
			{
				_current.State = ETouchState.Moving;
			}

			if (phase == TouchPhase.None || phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
			{
				_current.State = ETouchState.Finished;
				_end = _current;
			}

			_history[0] = _current;
		}

		public void Pause()
		{
			if (_current.State == ETouchState.Paused || _current.State == ETouchState.Finished)
				return;

			if (_current.Frame == Time.frameCount)
			{
				_current.State = ETouchState.Paused;
				_history[0] = _current;
				_end = _current;
				return;
			}

			for (int i = _history.Length - 1; i > 0; --i)
			{
				_history[i] = _history[i - 1];
			}
			_history[0] = default;

			_previous = _current;
			_current  = default;
			_delta    = default;

			_current.ID       = _previous.ID + 1;
			_current.State    = ETouchState.Paused;
			_current.Frame    = Time.frameCount;
			_current.Time     = Time.unscaledTime;
			_current.Position = _previous.Position;

			_delta.IsValid  = true;
			_delta.Count    = _current.ID       - _previous.ID;
			_delta.Frames   = _current.Frame    - _previous.Frame;
			_delta.Time     = _current.Time     - _previous.Time;
			_delta.Position = _current.Position - _previous.Position;

			_history[0] = _current;
			_end = _current;
		}

		public void Resume()
		{
			if (_current.State != ETouchState.Paused)
				return;

			if (_current.Frame == Time.frameCount)
			{
				_current.State = ETouchState.Resumed;
				_history[0] = _current;
				_end = default;
				return;
			}

			for (int i = _history.Length - 1; i > 0; --i)
			{
				_history[i] = _history[i - 1];
			}
			_history[0] = default;

			_previous = _current;
			_current  = default;
			_delta    = default;

			_current.ID       = _previous.ID + 1;
			_current.State    = ETouchState.Resumed;
			_current.Frame    = Time.frameCount;
			_current.Time     = Time.unscaledTime;
			_current.Position = _previous.Position;

			_delta.IsValid  = true;
			_delta.Count    = _current.ID       - _previous.ID;
			_delta.Frames   = _current.Frame    - _previous.Frame;
			_delta.Time     = _current.Time     - _previous.Time;
			_delta.Position = _current.Position - _previous.Position;

			_history[0] = _current;
			_end = default;
		}

		public void Finish()
		{
			if (_current.State == ETouchState.Finished)
				return;

			if (_current.State != ETouchState.Paused)
			{
				Pause();
			}

			_current.State = ETouchState.Finished;
			_history[0] = _current;
			_end = _current;
		}
	}
}
