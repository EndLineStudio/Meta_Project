namespace Example
{
	using UnityEngine;

	public struct InputTouchDelta
	{
		// PUBLIC MEMBERS

		public bool    IsValid;
		public int     Count;
		public int     Frames;
		public float   Time;
		public Vector2 Position;
	}
}
