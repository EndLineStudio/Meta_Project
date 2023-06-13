namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Special VR player implementation which uses 2 KCC instances.
	/// Root KCC - moves your root (headset origin) in world space. This object.
	/// Visual KCC - moves in your headset local space. Separate networked object.
	/// </summary>
	[RequireComponent(typeof(KCC))]
	[OrderBefore(typeof(KCC))]
	[OrderAfter(typeof(BeforeHitboxManagerUpdater))]
	public sealed class VRPlayer : NetworkBehaviour
	{
		// PRIVATE MEMBERS

		[Networked]
		private KCC _visualKCC { get; set; }

		// Hands are networked so other players can see them. Head position/rotation is propagated to the Visual KCC and doesn't need to be synced.
		[Networked]
		private Vector3    _leftHandPosition  { get; set; }
		[Networked]
		private Quaternion _leftHandRotation  { get; set; }
		[Networked]
		private Vector3    _rightHandPosition { get; set; }
		[Networked]
		private Quaternion _rightHandRotation { get; set; }

		[SerializeField]
		private KCC   _visualPrefab;
		[SerializeField]
		private float _areaOfInterestRadius;

		private PlayerInput    _input;
		private PlayerVisual   _visual;
		private SceneCamera    _camera;
		private NetworkCulling _culling;
		private KCC            _rootKCC;

		private Interpolator<Vector3>    _leftHandPositionInterpolator;
		private Interpolator<Quaternion> _leftHandRotationInterpolator;
		private Interpolator<Vector3>    _rightHandPositionInterpolator;
		private Interpolator<Quaternion> _rightHandRotationInterpolator;

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			name = Object.InputAuthority.ToString();

			if (Object.HasStateAuthority == true)
			{
				// Visual KCC is spawned on state authority only and synced later in RefreshVisual().
				_visualKCC = Runner.Spawn(_visualPrefab, transform.position, transform.rotation, Object.InputAuthority);
				_visualKCC.Object.SetPlayerAlwaysInterested(Object.InputAuthority, true);
			}

			if (Object.HasInputAuthority == true)
			{
				_camera = Runner.SimulationUnityScene.GetComponent<SceneCamera>();
			}

			RefreshVisual();

			// Both KCCs are updated manually in specific order.
			_rootKCC.SetManualUpdate(true);

			_leftHandPositionInterpolator  = GetInterpolator<Vector3>(nameof(_leftHandPosition));
			_leftHandRotationInterpolator  = GetInterpolator<Quaternion>(nameof(_leftHandRotation));
			_rightHandPositionInterpolator = GetInterpolator<Vector3>(nameof(_rightHandPosition));
			_rightHandRotationInterpolator = GetInterpolator<Quaternion>(nameof(_rightHandRotation));
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if (_visualKCC != null && _visualKCC.Object != null)
			{
				// Visual KCC is owned by this object.
				Runner.Despawn(_visualKCC.Object);
			}

			_camera    = null;
			_visual    = null;
			_visualKCC = null;
		}

		public override sealed void FixedUpdateNetwork()
		{
			RefreshVisual();

			if (_visual == null)
				return;
			if (_culling.IsCulled == true)
				return;

			if (Object.IsProxy == true)
			{
				// Update both KCCs and exit
				_rootKCC.ManualFixedUpdate();
				_visualKCC.ManualFixedUpdate();
				return;
			}

			// For following lines, we should use Input.FixedInput only. This property holds input for fixed updates.

			// Clamp input look rotation delta
			Vector2 lookRotation      = _rootKCC.FixedData.GetLookRotation(true, true);
			Vector2 lookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, _input.FixedInput.LookRotationDelta, -90.0f, 90.0f);

			// Apply clamped look rotation delta
			_rootKCC.AddLookRotation(lookRotationDelta);

			// Calculate input direction based on recently updated look rotation (the change propagates internally also to KCCData.TransformRotation)
			Vector3 inputDirection = _rootKCC.FixedData.TransformRotation * new Vector3(_input.FixedInput.MoveDirection.x, 0.0f, _input.FixedInput.MoveDirection.y);
			_rootKCC.SetInputDirection(inputDirection);

			// Update the Root KCC
			_rootKCC.ManualFixedUpdate();

			// Calculate world space position and rotation offsets between Root KCC (world space) and head (local space)
			Vector3 positionOffset = _rootKCC.FixedData.TransformRotation * _input.FixedInput.HeadPosition.OnlyXZ();
			Vector3 rotationOffset = _input.FixedInput.HeadRotation.eulerAngles;

			// Apply calculated values on top of values from Root KCC
			_visualKCC.SetPosition(_rootKCC.FixedData.TargetPosition + positionOffset);
			_visualKCC.SetLookRotation(_rootKCC.FixedData.LookPitch + rotationOffset.x, _rootKCC.FixedData.LookYaw + rotationOffset.y);

			// Update the Visual KCC
			_visualKCC.ManualFixedUpdate();

			// Store hands position for network synchronization
			_leftHandPosition  = _input.FixedInput.LeftHandPosition;
			_leftHandRotation  = _input.FixedInput.LeftHandRotation;
			_rightHandPosition = _input.FixedInput.RightHandPosition;
			_rightHandRotation = _input.FixedInput.RightHandRotation;

			// Update visual
			_visual.LeftHand.localPosition  = _leftHandPosition;
			_visual.LeftHand.localRotation  = _leftHandRotation;
			_visual.RightHand.localPosition = _rightHandPosition;
			_visual.RightHand.localRotation = _rightHandRotation;

			// Set AoI position
			Runner.AddPlayerAreaOfInterest(Object.InputAuthority, _rootKCC.FixedData.TargetPosition, _areaOfInterestRadius);

			if (_input.WasActivated(EGameplayInputAction.LeftTrigger) == true)
			{
				// Left trigger button action
			}

			if (_input.WasActivated(EGameplayInputAction.RightTrigger) == true)
			{
				// Right trigger button action
			}

			// Additional input processing goes here
		}

		public override sealed void Render()
		{
			RefreshVisual();

			if (_visual == null)
				return;
			if (_culling.IsCulled == true)
				return;

			if (Object.HasInputAuthority == true)
			{
				// For following lines, we should use Input.RenderInput and Input.CachedInput only. These properties hold input for render updates.
				// Input.RenderInput holds input for current render frame.
				// Input.CachedInput holds combined input for all render frames from last fixed update. This property will be used to set input for next fixed update (polled by Fusion).

				// Look rotation have to be updated to get smooth camera rotation

				// Get look rotation from last fixed update (not last render!)
				Vector2 lookRotation = _rootKCC.FixedData.GetLookRotation(true, true);

				// For correct look rotation, we have to apply deltas from all render frames since last fixed update => stored in Input.CachedInput
				Vector2 lookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, _input.CachedInput.LookRotationDelta, -90.0f, 90.0f);

				_rootKCC.SetLookRotation(lookRotation + lookRotationDelta);

				// Update the Root KCC
				_rootKCC.ManualRenderUpdate();

				// Calculate world space position and rotation offsets between Root KCC (world space) and head (local space)
				Vector3 positionOffset = _rootKCC.RenderData.TransformRotation * _input.RenderInput.HeadPosition.OnlyXZ();
				Vector3 rotationOffset = _input.RenderInput.HeadRotation.eulerAngles;

				// Apply calculated values on top of values from Root KCC
				_visualKCC.SetPosition(_rootKCC.RenderData.TargetPosition + positionOffset);
				_visualKCC.SetLookRotation(_rootKCC.RenderData.LookPitch + rotationOffset.x, _rootKCC.RenderData.LookYaw + rotationOffset.y);

				// Update the Visual KCC
				_visualKCC.ManualRenderUpdate();
			}
			else
			{
				// Update both KCCs
				_rootKCC.ManualRenderUpdate();
				_visualKCC.ManualRenderUpdate();

				// Interpolate hands for others
				_visual.LeftHand.localPosition  = _leftHandPositionInterpolator.Value;
				_visual.LeftHand.localRotation  = _leftHandRotationInterpolator.Value;
				_visual.RightHand.localPosition = _rightHandPositionInterpolator.Value;
				_visual.RightHand.localRotation = _rightHandRotationInterpolator.Value;
			}
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_input   = gameObject.GetComponent<PlayerInput>();
			_culling = gameObject.GetComponent<NetworkCulling>();
			_rootKCC = gameObject.GetComponent<KCC>();

			_culling.Updated = OnCullingUpdated;
		}

		private void LateUpdate()
		{
			if (Object == null || Object.HasInputAuthority == false)
				return;

			// Refresh VR pose and camera for local player.

			VRPose vrPose = VRPose.Get();

			if (_visual != null)
			{
				_visual.LeftHand.position  = _rootKCC.RenderData.TargetPosition + _rootKCC.RenderData.TransformRotation * vrPose.LeftHandPosition;
				_visual.LeftHand.rotation  = _rootKCC.RenderData.TransformRotation * vrPose.LeftHandRotation;
				_visual.RightHand.position = _rootKCC.RenderData.TargetPosition + _rootKCC.RenderData.TransformRotation * vrPose.RightHandPosition;
				_visual.RightHand.rotation = _rootKCC.RenderData.TransformRotation * vrPose.RightHandRotation;
			}

			if (_camera != null)
			{
				Vector3    cameraPosition = _rootKCC.RenderData.TargetPosition + _rootKCC.RenderData.TransformRotation * vrPose.HeadPosition;
				Quaternion cameraRotation = _rootKCC.RenderData.TransformRotation * vrPose.HeadRotation;

				_camera.SetPositionAndRotation(cameraPosition, cameraRotation);
			}
		}

		// PRIVATE METHODS

		private void RefreshVisual()
		{
			if (_visualKCC == null || _visualKCC.Object == null)
			{
				_visual = null;
				return;
			}

			if (_visualKCC.HasManualUpdate == false)
			{
				// Both KCCs are updated manually in specific order.
				_visualKCC.SetManualUpdate(true);
			}

			if (_visual == null)
			{
				_visual = _visualKCC.GetComponent<PlayerVisual>();
				_visual.name = $"{name} Visual";

				if (Object.HasInputAuthority == true)
				{
					// Only hands are visible to local player.
					_visual.SetVisibility(false, false, true);
				}
			}
		}

		private void OnCullingUpdated(bool isCulled)
		{
			// Show/hide the game object based on AoI (Area of Interest)

			if (_rootKCC.Collider != null)
			{
				_rootKCC.Collider.gameObject.SetActive(isCulled == false);
			}

			if (_visualKCC != null && _visualKCC.Collider != null)
			{
				_visualKCC.Collider.gameObject.SetActive(isCulled == false);
			}

			if (_visual != null)
			{
				_visual.SetVisibility(isCulled == false);
			}
		}
	}
}
