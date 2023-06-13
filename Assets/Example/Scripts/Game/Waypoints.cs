namespace Example
{
	using UnityEngine;

	/// <summary>
	/// Component for waypoints lookup, expected two level nesting (1st level is group, 2nd level is actual waypoint)
	/// </summary>
	public sealed class Waypoints : MonoBehaviour
	{
		// PUBLIC METHODS

		public Transform GetWaypoint(int group, int id)
		{
			return transform.GetChild(group).GetChild(id);
		}

		public Transform GetRandomWaypoint(out int group, out int id)
		{
			group = Random.Range(0, transform.childCount);

			Transform groupTransform = transform.GetChild(group);

			id = Random.Range(0, groupTransform.childCount);

			Transform waypointTransform = groupTransform.GetChild(id);

			return waypointTransform;
		}

		public Transform GetRandomWaypoint()
		{
			return GetRandomWaypoint(out int group, out int id);
		}
	}
}
