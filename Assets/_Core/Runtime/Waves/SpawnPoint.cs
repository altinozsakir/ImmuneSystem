using UnityEngine;

namespace Core.Waves
{
    public class SpawnPoint : MonoBehaviour
    {
        [Tooltip("Relative probability weight when choosing random spawn point.")]
        [Min(0.01f)] public float weight = 1f;

        [Tooltip("Optional: lane/group label (e.g., 'North', 'East').")]
        public string lane = "Default";

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.35f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1.0f);
        }
    }
}