namespace Example
{
	using UnityEngine;

	/// <summary>
	/// Wrapper for controlling player visual configuration and visibility.
	/// Affected by view (1st/3rd person), platform (VR/Standalone/Mobile) and culling state.
	/// </summary>
	public sealed class PlayerVisual : MonoBehaviour
	{
		// PUBLIC MEMBERS

		public Transform Root      => _root;
		public Transform Head      => _head;
		public Transform Body      => _body;
		public Transform LeftHand  => _leftHand;
		public Transform RightHand => _rightHand;

		// PRIVATE MEMBERS

		[SerializeField]
		private Transform _root;
		[SerializeField]
		private Transform _head;
		[SerializeField]
		private Transform _body;
		[SerializeField]
		private Transform _leftHand;
		[SerializeField]
		private Transform _rightHand;

		// PUBLIC METHODS

		public void SetVisibility(bool isVisible)
		{
			SetVisibility(isVisible, isVisible, isVisible);
		}

		public void SetVisibility(bool headVisible, bool bodyVisible, bool handsVisible)
		{
			_head.gameObject.SetActive(headVisible);
			_body.gameObject.SetActive(bodyVisible);
			_leftHand.gameObject.SetActive(handsVisible);
			_rightHand.gameObject.SetActive(handsVisible);
		}
	}
}
