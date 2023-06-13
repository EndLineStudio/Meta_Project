namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Base class for Simple and Advanced player implementations.
	/// Provides references to components and basic setup.
	/// </summary>
	[RequireComponent(typeof(KCC))]
	[RequireComponent(typeof(PlayerVisual))]
	[OrderBefore(typeof(KCC))]
	[OrderAfter(typeof(BeforeHitboxManagerUpdater))]
	public abstract class Player : NetworkKCCProcessor
	{
		// PUBLIC MEMBERS

		public KCC            KCC     => _kcc;
		public PlayerInput    Input   => _input;
		public PlayerVisual   Visual  => _visual;
		public SceneCamera    Camera  => _camera;
		public NetworkCulling Culling => _culling;

		[Networked]
		public float SpeedMultiplier { get; set; } = 1.0f;

		// PROTECTED MEMBERS

		protected Transform        CameraPivot    => _cameraPivot;
		protected Transform        CameraHandle   => _cameraHandle;
		protected float            MaxCameraAngle => _maxCameraAngle;
		protected Vector3          JumpImpulse    => _jumpImpulse;
		protected DashKCCProcessor DashProcessor  => _dashProcessor;

		// PRIVATE MEMBERS

		[SerializeField]
		private Transform        _cameraPivot;
		[SerializeField]
		private Transform        _cameraHandle;
		[SerializeField]
		private float            _maxCameraAngle;
		[SerializeField]
		private float            _areaOfInterestRadius;
		[SerializeField]
		private Vector3          _jumpImpulse;
		[SerializeField]
		private DashKCCProcessor _dashProcessor;

		private KCC            _kcc;
		private PlayerInput    _input;
		private PlayerVisual   _visual;
		private SceneCamera    _camera;
		private NetworkCulling _culling;
		private float          _cameraDistance;

		// PUBLIC METHODS

		/// <summary>
		/// Called from menu to speed up character for faster navigation through example levels.
		/// Players should not be able to define their speed unless this is a design decision.
		/// </summary>
		[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
		public void ToggleSpeedRPC(int direction)
		{
			if (direction > 0)
			{
				SpeedMultiplier *= 2.0f;
				if (SpeedMultiplier >= 10.0f)
				{
					SpeedMultiplier = 0.25f;
				}
			}
			else
			{
				SpeedMultiplier *= 0.5f;
				if (SpeedMultiplier <= 0.2f)
				{
					SpeedMultiplier = 8.0f;
				}
			}
		}

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			name = Object.InputAuthority.ToString();

			if (Object.HasInputAuthority == true)
			{
				_camera = Runner.SimulationUnityScene.GetComponent<SceneCamera>();
			}

			// Explicit KCC initialization. This needs to be called before using API, otherwise changes could be overriden by implicit initialization from KCC.Start() or KCC.Spawned()
			_kcc.Initialize(EKCCDriver.Fusion);

			// Player itself can modify kinematic speed, registering to KCC
			_kcc.AddModifier(this);
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			_camera = null;
		}

		public override void FixedUpdateNetwork()
		{
			// By default we expect derived classes to process input in FixedUpdateNetwork().
			// The correct approach is to set input before KCC updates internally => we need to specify [OrderBefore(typeof(KCC))] attribute.

			// SimplePlayer runs input processing in FixedUpdateNetwork() as expected, but KCC runs its internal update after Player.FixedUpdateNetwork().
			// Following call sets AoI position to last fixed update KCC position. It should not be a problem in most cases, but some one-frame glitches after teleporting might occur.
			// This problem is solved in AdvancedPlayer which uses manual KCC update at the cost of slightly increased complexity.

			Runner.AddPlayerAreaOfInterest(Object.InputAuthority, _kcc.FixedData.TargetPosition, _areaOfInterestRadius);
		}

		// NetworkKCCProcessor INTERFACE

		// Lowest priority => this processor will be executed last.
		public override float Priority => float.MinValue;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			// Only SetKinematicSpeed stage is used, rest are filtered out and corresponding method calls will be skipped.
			return EKCCStages.SetKinematicSpeed;
		}

		public override void SetKinematicSpeed(KCC kcc, KCCData data)
		{
			// Applying multiplier.
			data.KinematicSpeed *= SpeedMultiplier;
		}

		// PROTECTED METHODS

		protected void RefreshCamera(bool collideWithGeometry)
		{
			if (Camera == null || CameraPivot == null || CameraHandle == null)
				return;

			// Default is CameraHandle transform.
			Vector3    cameraPosition = CameraHandle.position;
			Quaternion cameraRotation = CameraHandle.rotation;

			// Checking collision with geometry so the Camera transform is not pushed inside.
			if (collideWithGeometry == true)
			{
				Vector3 raycastPosition  = CameraPivot.position;
				Vector3 raycastDirection = CameraHandle.position - raycastPosition;
				float   raycastDistance  = raycastDirection.magnitude;

				if (raycastDistance > 0.001f)
				{
					raycastDirection /= raycastDistance;

					PhysicsScene physicsScene = Runner.GetPhysicsScene();
					if (physicsScene.SphereCast(raycastPosition, 0.1f, raycastDirection, out RaycastHit hitInfo, raycastDistance, -1, QueryTriggerInteraction.Ignore) == true)
					{
						float hitCameraDistance = Mathf.Max(0.0f, hitInfo.distance - 0.25f);
						if (hitCameraDistance < _cameraDistance)
						{
							_cameraDistance = hitCameraDistance;
						}
					}

					cameraPosition = raycastPosition + raycastDirection * Mathf.Min(_cameraDistance, raycastDistance);
				}
			}

			Camera.SetPositionAndRotation(cameraPosition, cameraRotation);
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_kcc     = gameObject.GetComponent<KCC>();
			_input   = gameObject.GetComponent<PlayerInput>();
			_visual  = gameObject.GetComponent<PlayerVisual>();
			_culling = gameObject.GetComponent<NetworkCulling>();

			_culling.Updated = OnCullingUpdated;
		}

		private void Update()
		{
			_cameraDistance += Time.deltaTime * 8.0f;
		}

		// PRIVATE METHODS

		private void OnCullingUpdated(bool isCulled)
		{
			// Show/hide the game object based on AoI (Area of Interest)

			_visual.SetVisibility(isCulled == false);

			if (_kcc.Collider != null)
			{
				_kcc.Collider.gameObject.SetActive(isCulled == false);
			}
		}
	}
}
