using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentDebug : MonoBehaviour
{
    NavMeshAgent agent;
    void Awake() => agent = GetComponent<NavMeshAgent>();
    void OnDrawGizmos()
    {
        if (agent && agent.hasPath)
        {
            Gizmos.color = Color.cyan;
            var path = agent.path;
            for (int i = 0; i < path.corners.Length - 1; i++)
                Gizmos.DrawLine(path.corners[i], path.corners[i+1]);
        }
    }
}
