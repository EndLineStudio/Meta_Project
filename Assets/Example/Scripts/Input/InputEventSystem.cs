namespace Example
{
	using UnityEngine.EventSystems;

	public sealed partial class InputEventSystem : EventSystem
	{
		// PUBLIC METHODS

		public void UpdateEventSystem()
		{
			base.Update();
		}

		// EventSystem INTERFACE

		protected override void Update()
		{
		}
	}
}
