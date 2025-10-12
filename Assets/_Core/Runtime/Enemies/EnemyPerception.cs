using UnityEngine;
using Core.Combat;
using System.Collections.Generic;
using Core.Structures; // Add this line if StructureHealth is in Core.Structures namespace

namespace Core.Enemies
{

    [RequireComponent(typeof(SphereCollider))]
    public class EnemyPerception : MonoBehaviour
    {
        public readonly List<ThreatProfile> Seen = new();
        [SerializeField] private LayerMask interestMask; // set to towers/walls/goal layers
        [SerializeField] private float pruneInterval = 0.25f;
        float _nextPrune;
        void OnEnable() => ThreatProfile.OnAnyThreatDestroyed += HandleThreatDestroyed;
        void OnDisable() => ThreatProfile.OnAnyThreatDestroyed -= HandleThreatDestroyed;
        void OnTriggerEnter(Collider other)
        {

            if (((1 << other.gameObject.layer) & interestMask) == 0) return;
            if (other.TryGetComponent(out ThreatProfile tp) && tp.IsAlive && tp.Team == Team.Player)
                if (!Seen.Contains(tp)) Seen.Add(tp);
            if (other.TryGetComponent<StructureHealth>(out var sh))
                sh.OnDied += HandleDied;
        }
        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<ThreatProfile>(out var t))
                Seen.Remove(t);
        }
        // Optional: prune dead each few seconds
        public void PruneDead()
        {
        if (Time.time < _nextPrune) return;
        _nextPrune = Time.time + pruneInterval;
        for (int i = Seen.Count - 1; i >= 0; i--)
            if (Seen[i] == null || !Seen[i].gameObject.activeInHierarchy)
                Seen.RemoveAt(i);
        }
        void Update()
        {
            if (Time.time < _nextPrune) return;
            _nextPrune = Time.time + pruneInterval;
            PruneDead();
        }
        void HandleDied(StructureHealth sh)
        {
            // remove quickly; OnTriggerExit won't fire after Destroy
            for (int i = Seen.Count - 1; i >= 0; i--)
                if (Seen[i] is Component c && c == sh) Seen.RemoveAt(i);
        }
        void HandleThreatDestroyed(ThreatProfile tp)
        {
            // Types match -> no cast needed, no compile error
            Seen.Remove(tp);
        }
    }


}