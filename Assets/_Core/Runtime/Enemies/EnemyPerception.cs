using UnityEngine;
using Core.Combat;
using System.Collections.Generic;

namespace Core.Enemies
{
    
[RequireComponent(typeof(SphereCollider))]
public class EnemyPerception : MonoBehaviour
{
    public readonly List<ThreatProfile> Seen = new();
    [SerializeField] private LayerMask interestMask; // set to towers/walls/goal layers

    void OnTriggerEnter(Collider other)
    {
            Debug.Log("Perception Triggred Collider Entered!");
        if (((1 << other.gameObject.layer) & interestMask) == 0) return;
        if (other.TryGetComponent<ThreatProfile>(out var t) && t.IsAlive && t.Team == Team.Player)
                Debug.Log("Thread Profile found!");
                if (!Seen.Contains(t)) Seen.Add(t);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ThreatProfile>(out var t))
            Seen.Remove(t);
    }
    // Optional: prune dead each few seconds
    public void PruneDead()
    {
        for (int i = Seen.Count - 1; i >= 0; i--) if (!Seen[i].IsAlive) Seen.RemoveAt(i);
    }
}


}