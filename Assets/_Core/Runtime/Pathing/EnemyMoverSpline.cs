// Assets/_Core/Runtime/Pathing/EnemyMoverSpline.cs
using UnityEngine;
using UnityEngine.Splines;


namespace Core.Pathing
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMoverSpline : MonoBehaviour
    {
        public SplineContainer lane;
        public float speed = 3.5f;
        public bool loop = false;
        public bool orientToTangent = true;
        public float heightOffset = 0.0f; // keep feet above ground if needed


        Rigidbody _rb;
        Spline _spline;
        double _length;
        double _distance;


        public float DistanceToGoal => (float)Mathf.Max(0f, (float)(_length - _distance));
        public float NormalizedT => _length > 0.0001 ? (float)(_distance / _length) : 0f;

                    bool _paused;
public void SetPaused(bool paused) => _paused = paused;
        void Awake() => _rb = GetComponent<Rigidbody>();
        void OnEnable() { if (_rb) _rb.interpolation = RigidbodyInterpolation.Interpolate; }


        void Start()
        {
            ResolveSpline();
            ResetToStart();
        }


        void ResolveSpline()
        {
            _spline = null; _length = 0; _distance = 0;
            if (!lane) return;
            if (lane.Splines != null && lane.Splines.Count > 0) _spline = lane.Splines[0];
#if UNITY_600_0_OR_NEWER
else _spline = lane.Spline; // fallback if single-spline API is present
#endif
            if (_spline != null) _length = SplineUtility.CalculateLength(_spline, lane != null ? lane.transform.localToWorldMatrix : Matrix4x4.identity);
        }


        public void ResetToStart()
        {
            _distance = 0;
            if (_spline == null) return;
            SetPoseAtDistance(_distance);
        }


        void FixedUpdate()
        {
            if (_paused) return;

            if (_spline == null || _length <= 0) return;
            _distance += speed * Time.fixedDeltaTime;

            
            if (_distance >= _length)
            {
                if (loop) _distance = 0.0f; else { _distance = _length; enabled = false; }
            }
            SetPoseAtDistance(_distance);
        }


        void SetPoseAtDistance(double dist)
        {
            float t = _length > 0 ? (float)(dist / _length) : 0f;
            Vector3 localPos = SplineUtility.EvaluatePosition(_spline, t);
            Vector3 worldPos = lane.transform.TransformPoint(localPos);
            worldPos.y += heightOffset; // keep flat Y if desired


            if (orientToTangent)
            {
                Vector3 localTan = SplineUtility.EvaluateTangent(_spline, t);
                Vector3 worldTan = lane.transform.TransformDirection(localTan);
                if (worldTan.sqrMagnitude > 0.0001f)
                    _rb.MoveRotation(Quaternion.LookRotation(new Vector3(worldTan.x, 0, worldTan.z), Vector3.up));
            }


            // lock to XZ plane by preserving current Y if needed
            var pos = _rb.position; worldPos.y = (heightOffset != 0f) ? worldPos.y : pos.y;
            _rb.MovePosition(new Vector3(worldPos.x, worldPos.y, worldPos.z));
        }
    }
}