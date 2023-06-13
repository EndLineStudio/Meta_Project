namespace Example
{
	using UnityEngine;
	using Fusion;

	using Stopwatch = System.Diagnostics.Stopwatch;

	/// <summary>
	/// Script for testing performance of CCs.
	/// </summary>
	public sealed class PerformanceTest : NetworkBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private float     _speed;
		[SerializeField]
		private Waypoints _waypoints;
		[SerializeField]
		private TestCC    _prefab;
		[SerializeField]
		private int       _count;
		[SerializeField]
		private bool      _log;

		private Stopwatch _timer = new Stopwatch();
		private TestCC[]  _instances;

		// NetworkBehaviour INTERFACE

		public override void Spawned()
		{
			if (Object.HasStateAuthority == false)
				return;

			// Spawn CCs and assign target locations.

			_instances = new TestCC[_count];

			for (int i = 0; i < _count; ++i)
			{
				Transform spawnPoint = _waypoints.GetRandomWaypoint();

				TestCC instance = Runner.Spawn(_prefab, spawnPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), 1.0f, Random.Range(-0.5f, 0.5f)), spawnPoint.rotation);
				instance.SetManualUpdate(true);
				instance.SetTarget(_waypoints.GetRandomWaypoint());
				instance.SetSpeed(_speed);

				_instances[i] = instance;
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (Object.HasStateAuthority == false)
				return;

			// Manually update all CCs (fixed update) and assign new target location if needed.

			_timer.Restart();

			for (int i = 0, count = _instances.Length; i < count; ++i)
			{
				TestCC instance = _instances[i];
				instance.ManualFixedUpdate();
				if (instance.HasTarget == false)
				{
					instance.SetTarget(_waypoints.GetRandomWaypoint());
				}
			}

			_timer.Stop();

			if (_log == true)
			{
				Debug.LogWarning($"[F][{Time.frameCount}]: {_timer.Elapsed.TotalMilliseconds.ToString("F3")}ms");
			}
		}

		public override void Render()
		{
			if (Object.HasStateAuthority == false)
				return;

			// Manually update all CCs (render update).

			_timer.Restart();

			for (int i = 0, count = _instances.Length; i < count; ++i)
			{
				TestCC instance = _instances[i];
				instance.ManualRenderUpdate();
			}

			_timer.Stop();

			if (_log == true)
			{
				Debug.Log($"[R][{Time.frameCount}]: {_timer.Elapsed.TotalMilliseconds.ToString("F3")}ms");
			}
		}
	}
}
