// Assets/_Core/Runtime/Spawn/EnemySpawnerSpline.cs
using UnityEngine;
using UnityEngine.Splines;
using Core.Pathing;


namespace Core.Spawn
{
public class EnemySpawnerSpline : MonoBehaviour
{
public GameObject enemyPrefab;
public SplineContainer lane;
public int count = 12;
public float interval = 0.75f;
public float startHeight = 1.0f; // center height for capsule
public Transform container;


float _timer; int _spawned;


void Update()
{
if (!enemyPrefab || !lane) return;
if (_spawned >= count) return;


_timer -= Time.deltaTime;
if (_timer <= 0f)
{
_timer = interval;
SpawnOne();
}
}


void SpawnOne()
{
// Evaluate lane start (t=0)
var spl = (lane.Splines != null && lane.Splines.Count > 0) ? lane.Splines[0] : lane.Spline;
Vector3 local = SplineUtility.EvaluatePosition(spl, 0f);
Vector3 world = lane.transform.TransformPoint(local);
world.y = startHeight;


var parent = container ? container : transform;
var go = Instantiate(enemyPrefab, world, Quaternion.identity, parent);
go.tag = "Enemy"; // optional


if (go.TryGetComponent<EnemyMoverSpline>(out var mover))
{
mover.lane = lane;
mover.ResetToStart();
mover.enabled = true;
}


_spawned++;
}
}
}