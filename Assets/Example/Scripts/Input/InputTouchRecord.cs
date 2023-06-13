namespace Example
{
	using UnityEngine;

	public enum ETouchState
	{
		None       = 0,
		Started    = 1,
		Stationary = 2,
		Moving     = 3,
		Paused     = 4,
		Resumed    = 5,
		Finished   = 6,
	}

	public struct InputTouchRecord
	{
		// PUBLIC MEMBERS

		public int         ID;
		public ETouchState State;
		public int         Frame;
		public float       Time;
		public Vector2     Position;

		public bool IsValid => State != ETouchState.None;
	}
}
