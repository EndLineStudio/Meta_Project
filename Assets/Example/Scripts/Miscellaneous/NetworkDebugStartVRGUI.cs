namespace Example
{
	using UnityEngine;

	[RequireComponent(typeof(NetworkDebugStart))]
	public sealed class NetworkDebugStartVRGUI : Fusion.Behaviour
	{
		[SerializeField]
		private Canvas    _canvas;
		[SerializeField]
		private Transform _leftHand;
		[SerializeField]
		private Transform _rightHand;
		[SerializeField]
		private Transform _interactionManager;

		private Vector3    _basePosition;
		private Quaternion _baseRotation;

		public void StartSinglePlayer()
		{
			SetVisibility(false);
			GetComponent<NetworkDebugStart>().StartSinglePlayer();
		}

		public void StartSharedClient()
		{
			SetVisibility(false);
			GetComponent<NetworkDebugStart>().StartSharedClient();
		}

		public void StartHost()
		{
			SetVisibility(false);
			GetComponent<NetworkDebugStart>().StartHost();
		}

		public void StartServer()
		{
			SetVisibility(false);
			GetComponent<NetworkDebugStart>().StartServer();
		}

		public void StartClient()
		{
			SetVisibility(false);
			GetComponent<NetworkDebugStart>().StartClient();
		}

		public void StartAutoHostOrClient()
		{
			SetVisibility(false);
			GetComponent<NetworkDebugStart>().StartAutoClient();
		}

		private void Awake()
		{
			bool isVREnabled = ApplicationUtility.IsVREnabled();

			SetVisibility(isVREnabled);
			GetComponent<NetworkDebugStartGUI>().enabled = isVREnabled == false;
		}

		private void Start()
		{
			if (_canvas.enabled == false)
				return;

			Camera    camera          = Camera.main;
			Transform cameraTransform = camera.transform;

			_basePosition = cameraTransform.position - cameraTransform.rotation * new Vector3(0.0f, 1.5f, 0.0f);
			_baseRotation = cameraTransform.rotation;

			_interactionManager.position = _basePosition;
			_interactionManager.rotation = _baseRotation;

			_canvas.transform.position   = _basePosition + _baseRotation * new Vector3(0.0f, 1.5f, 10.0f);
			_canvas.transform.rotation   = _baseRotation;
			_canvas.transform.localScale = Vector3.one * 0.01f;

			_canvas.worldCamera = camera;
		}

		private void Update()
		{
			if (_canvas.enabled == false)
				return;

			VRPose pose = VRPose.Get();

			Transform cameraTransform = Camera.main.transform;
			cameraTransform.position = _basePosition + _baseRotation * pose.HeadPosition;
			cameraTransform.rotation = _baseRotation * pose.HeadRotation;

			_leftHand.position = _basePosition + _baseRotation * pose.LeftHandPosition;
			_leftHand.rotation = _baseRotation * pose.LeftHandRotation;

			_rightHand.position = _basePosition + _baseRotation * pose.RightHandPosition;
			_rightHand.rotation = _baseRotation * pose.RightHandRotation;
		}

		private void SetVisibility(bool isVisible)
		{
			_canvas.enabled = isVisible;
			_canvas.gameObject.SetActive(isVisible);

			_leftHand.gameObject.SetActive(isVisible);
			_rightHand.gameObject.SetActive(isVisible);
			_interactionManager.gameObject.SetActive(isVisible);
		}
	}
}
