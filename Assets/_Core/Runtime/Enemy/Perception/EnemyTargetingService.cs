using UnityEngine;

namespace Core.Enemy.Perception
{
    public class EnemyTargetingService
    {
        private readonly Transform self;
        private readonly LayerMask mask;

        public EnemyTargetingService(Transform self, LayerMask mask)
        {
            this.self = self;
            this.mask = mask;
        }

        // v1: just returns objective transform position
        public bool TryGetObjective(Transform objective, out Vector3 pos)
        {
            pos = default;
            if (!objective) return false;
            pos = objective.position;
            return true;
        }

        // v2 (optional): nearest damageable GameObject within radius (simulation)
        public GameObject FindNearestDamageable(float radius)
        {
            var hits = Physics.OverlapSphere(self.position, radius, mask, QueryTriggerInteraction.Ignore);
            float best = float.PositiveInfinity;
            GameObject bestGo = null;

            for (int i = 0; i < hits.Length; i++)
            {
                var go = hits[i].gameObject;
                float d = (go.transform.position - self.position).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    bestGo = go;
                }
            }
            return bestGo;
        }
    }
}