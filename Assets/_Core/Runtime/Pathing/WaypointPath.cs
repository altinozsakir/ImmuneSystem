using UnityEngine;


namespace Core.Pathing
{
    [ExecuteAlways]
    public class WaypointPath : MonoBehaviour
    {
        public Transform[] points; // Order = travel order
        public bool loop = false;


        public int Count => points == null ? 0 : points.Length;
        public Transform GetPoint(int index) => (Count == 0) ? null : points[Mathf.Clamp(index, 0, Count - 1)];


        private void OnDrawGizmos()
        {
            if (Count < 2) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < Count - 1; i++)
            {
                if (!points[i] || !points[i + 1]) continue;
                Gizmos.DrawSphere(points[i].position, 0.2f);
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
            Gizmos.DrawSphere(points[Count - 1].position, 0.2f);
            if (loop && points[0] && points[Count - 1])
                Gizmos.DrawLine(points[Count - 1].position, points[0].position);
        }
    }
}