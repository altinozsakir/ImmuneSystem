using UnityEngine;

namespace Core.Pathing
{
    public class NavWaypointFollower : MonoBehaviour
    {
        public Transform[] waypoints;  // inject from spawner after spawn
        public float nextAtDistance = 1.2f;

        int _idx;
        NavAgentMover _mover;

        void Awake(){ _mover = GetComponent<NavAgentMover>(); }

        public void SetRoute(Transform[] wps)
        {
            waypoints = wps;
            _idx = 0;
            if (_mover && waypoints != null && waypoints.Length > 0)
                _mover.SetDestination(waypoints[0].position);
        }

        void Update()
        {
            if (waypoints == null || waypoints.Length == 0 || _mover == null) return;
            var dest = waypoints[_idx].position;
            if (Vector3.SqrMagnitude(transform.position - dest) <= nextAtDistance * nextAtDistance)
            {
                _idx++;
                if (_idx < waypoints.Length) _mover.SetDestination(waypoints[_idx].position);
                else if (_mover.goal) _mover.SetDestination(_mover.goal.position);
            }
        }
    }
}
