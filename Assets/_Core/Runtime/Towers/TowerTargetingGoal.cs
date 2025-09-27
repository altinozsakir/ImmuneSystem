using UnityEngine;
using Core.Pathing; // EnemyMoverSpline

namespace Core.Towers
{
public class TowerTargetingGoal : MonoBehaviour
{
    public TowerBase tower;
    public LayerMask enemyMask;
    public float reacquireInterval = 0.1f;
    public int maxColliders = 32; // buffer size for non-alloc overlap


    Transform _current;
    float _timer;
    Collider[] _buffer;


    public Transform Current => _current;


    void Awake()
    {
        if (!tower) tower = GetComponent<TowerBase>();
        _buffer = new Collider[Mathf.Max(4, maxColliders)];
    }


    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = reacquireInterval;
            _current = AcquireClosestToGoal();
        }


        // Optional yaw
        if (_current && tower && tower.yawPivot)
        {
            Vector3 to = _current.position - tower.yawPivot.position; to.y = 0f;
            if (to.sqrMagnitude > 0.0001f)
                tower.yawPivot.rotation = Quaternion.Lerp(
                tower.yawPivot.rotation,
                Quaternion.LookRotation(to, Vector3.up),
                Time.deltaTime * 10f
                );
        }
    }


    Transform AcquireClosestToGoal()
    {
        if (!tower) return null;
        int hits = Physics.OverlapSphereNonAlloc(transform.position, tower.range, _buffer, enemyMask, QueryTriggerInteraction.Ignore);
        Transform best = null; float bestDist = float.MaxValue;
        for (int i = 0; i < hits; i++)
        {
            var col = _buffer[i];
            if (!col || !col.gameObject.activeInHierarchy) continue;
            var t = col.attachedRigidbody ? col.attachedRigidbody.transform : col.transform;
            if (!t) continue;
            var mover = t.GetComponent<EnemyMoverSpline>();
            if (!mover) continue;
            float d = mover.DistanceToGoal;
            if (d < bestDist) { bestDist = d; best = t; }
        }
        return best;
    }
}
}