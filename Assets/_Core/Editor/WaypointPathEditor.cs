#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Core.Pathing;

[CustomEditor(typeof(WaypointPath))]
public class WaypointPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var path = (WaypointPath)target;

        if (GUILayout.Button("Snap Points To Ground"))
        {
            foreach (var t in path.points)
            {
                if (!t) continue;
                var pos = t.position + Vector3.up * 50f;
                if (Physics.Raycast(pos, Vector3.down, out var hit, 200f))
                    t.position = hit.point;
            }
        }
    }
}
#endif