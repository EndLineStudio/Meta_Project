namespace Example
{
	using System;
	using System.Collections.Generic;
	using UnityEngine.InputSystem;
	using UnityEngine.InputSystem.Controls;

	public sealed partial class InputTouches
	{
		// PUBLIC MEMBERS

		public Action<InputTouch> TouchStarted;
		public Action<InputTouch> TouchFinished;

		// PRIVATE MEMBERS

		private int              _inputTouchID;
		private List<InputTouch> _activeTouches  = new List<InputTouch>();
		private List<InputTouch> _missingTouches = new List<InputTouch>();

		// PUBLIC METHODS

		public void Update()
		{
			Touchscreen touchScreen = Touchscreen.current;
			if (touchScreen == null)
				return;

			_missingTouches.Clear();
			_missingTouches.AddRange(_activeTouches);

			for (int i = 0, count = touchScreen.touches.Count; i < count; ++i)
			{
				TouchControl touchControl = touchScreen.touches[i];

				InputTouch activeTouch = GetActiveTouch(touchControl.touchId.ReadValue());
				if (activeTouch == null)
				{
					if (touchControl.isInProgress == false)
						continue;

					++_inputTouchID;

					activeTouch = new InputTouch(_inputTouchID, touchControl);

					_activeTouches.Add(activeTouch);
					TouchStarted.SafeInvoke(activeTouch);
				}
				else
				{
					_missingTouches.Remove(activeTouch);
				}

				if (activeTouch.Current.State == ETouchState.Finished)
					continue;

				activeTouch.Update(touchControl);

				if (activeTouch.Current.State == ETouchState.Finished)
				{
					_activeTouches.Remove(activeTouch);
					TouchFinished.SafeInvoke(activeTouch);
				}
			}

			for (int i = _missingTouches.Count - 1; i >= 0; --i)
			{
				InputTouch missingTouch = _missingTouches[i];
				missingTouch.Finish();

				_activeTouches.Remove(missingTouch);
				TouchFinished.SafeInvoke(missingTouch);
			}

			for (int i = _activeTouches.Count - 1; i >= 0; --i)
			{
				InputTouch activeTouch = _activeTouches[i];
				if (activeTouch.Current.State == ETouchState.Finished)
				{
					_activeTouches.RemoveAt(i);
					TouchFinished.SafeInvoke(activeTouch);
				}
			}
		}

		// PRIVATE METHODS

		private InputTouch GetActiveTouch(int touchID)
		{
			for (int i = 0, count = _activeTouches.Count; i < count; ++i)
			{
				InputTouch inputTouch = _activeTouches[i];
				if (inputTouch.TouchID == touchID)
					return inputTouch;
			}

			return null;
		}
	}
}
