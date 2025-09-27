using UnityEngine;

namespace Core.Pathing
{
    [RequireComponent(typeof(Rigidbody))]
    public class PathFollower : MonoBehaviour
    {
        public WaypointPath path;
        public float speed = 3.5f;
        public float arriveThreshold = 0.05f;
        public bool orientToVelocity = true;

        Rigidbody _rb;
        int _index;
        Vector3 _target;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        // IMPORTANT: do NOT touch path here; spawner may not have assigned it yet.
        void OnEnable() { /* no-op */ }

        // Start runs after the spawner finishes setting fields this frame
        void Start()
        {
            ResetToStart();
        }

        public void ResetToStart()
        {
            _index = 0;
            UpdateTargetSafe();
            // Disable if still no path — avoids NREs entirely
            enabled = (path != null && path.Count > 0);
        }

        void FixedUpdate()
        {
            if (path == null || path.Count == 0) return;

            Vector3 pos = _rb.position;
            Vector3 to = _target - pos; to.y = 0f;

            if (to.sqrMagnitude <= arriveThreshold * arriveThreshold)
            {
                Advance();
                to = _target - pos; to.y = 0f;
                if (to.sqrMagnitude <= 0.0001f) return;
            }

            Vector3 dir = to.normalized;
            _rb.MovePosition(pos + dir * speed * Time.fixedDeltaTime);

            if (orientToVelocity && dir.sqrMagnitude > 0.0001f)
                _rb.MoveRotation(Quaternion.LookRotation(dir, Vector3.up));
        }

        void Advance()
        {
            if (path == null || path.Count == 0) { enabled = false; return; }

            _index++;
            if (_index >= path.Count)
            {
                if (path.loop) _index = 0;
                else { enabled = false; return; }
            }
            UpdateTargetSafe();
        }

        void UpdateTargetSafe()
        {
            if (path == null || path.Count == 0)
            {
                _target = transform.position;
                return;
            }

            int clamped = Mathf.Clamp(_index, 0, path.Count - 1);
            var p = path.GetPoint(clamped);
            _target = p ? p.position : transform.position;
        }
    }
}
