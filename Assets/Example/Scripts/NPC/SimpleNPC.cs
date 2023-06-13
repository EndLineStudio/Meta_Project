namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Example of simple NPC - travelling between waypoints.
	/// </summary>
	[RequireComponent(typeof(KCC))]
	[OrderBefore(typeof(KCC))]
	[OrderAfter(typeof(NetworkCulling))]
	public sealed class SimpleNPC : NetworkBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private GameObject  _visual;
		[SerializeField]
		private Transform[] _waypoints;

		private KCC            _kcc;
		private NetworkCulling _culling;
		private int            _currentWaypoint;

		// NetworkBehaviour INTERFACE

		public override void FixedUpdateNetwork()
		{
			// Ignore if culled (out of AoI)
			if (_culling.IsCulled == true)
				return;

			SetDirection(EKCCDriver.Fusion, true);
		}

		public override void Render()
		{
			// Ignore if culled (out of AoI)
			if (_culling.IsCulled == true)
				return;

			SetDirection(EKCCDriver.Fusion, false);
		}

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			_kcc     = gameObject.GetComponent<KCC>();
			_culling = gameObject.GetComponent<NetworkCulling>();

			_culling.Updated = OnCullingUpdated;
		}

		private void FixedUpdate()
		{
			SetDirection(EKCCDriver.Unity, true);
		}

		private void Update()
		{
			SetDirection(EKCCDriver.Unity, false);
		}

		// PRIVATE METHODS

		private void SetDirection(EKCCDriver driver, bool updateWaypoint)
		{
			// NPC spawned via Runner.Spawn() will be driven by FUN/Render
			// NPC spawned via GameObject.Instantiate() will be driven by FixedUpdate/Update
			// This is just to highlight that KCC can seamlessly run without being networked - useable for local objects

			if (_kcc.Driver != driver)
				return;
			if (_waypoints.Length == 0)
				return;

			// Random waypoint selection, only in fixed update

			Vector3 direction = (_waypoints[_currentWaypoint].position - _kcc.Data.TargetPosition).OnlyXZ();
			if (updateWaypoint == true && direction.sqrMagnitude < 2.0f)
			{
				_currentWaypoint = (_currentWaypoint + 1) % _waypoints.Length;

				_kcc.Jump((Vector3.up + direction) * 5.0f);
			}

			// Setting KCC properties, these calls will be ignored on proxies

			_kcc.SetLookRotation(Quaternion.LookRotation(direction));
			_kcc.SetInputDirection(direction);
		}

		private void OnCullingUpdated(bool isCulled)
		{
			// Show/hide this object when culling status is updated

			_visual.SetActive(isCulled == false);

			if (_kcc.Collider != null)
			{
				_kcc.Collider.gameObject.SetActive(isCulled == false);
			}
		}
	}
}
