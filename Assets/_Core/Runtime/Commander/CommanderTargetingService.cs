using Core.Commander.Targeting;
using NUnit.Framework;
using UnityEngine;


namespace Core.Commander
{
    public class CommanderTargetingService{
        private readonly Transform origin;
        private readonly LayerMask mask;
    
        public CommanderTargetingService(Transform origin, LayerMask mask)
        {
            this.origin = origin;
            this.mask = mask;
        }


        public IDamageable FindNearestDamageable(float radius)
        {
            Collider[] hits = Physics.OverlapSphere(origin.position, radius, mask, QueryTriggerInteraction.Ignore);

            float best = float.PositiveInfinity;
            IDamageable bestTarget = null;


            for(int i=0; i < hits.Length; i++)
            {
                var go = hits[i].gameObject;

                var dmg =  go.GetComponentInParent<IDamageable>();
                if(dmg == null || !dmg.IsAlive) continue;

                float d = (dmg.Transform.position - origin.position).sqrMagnitude;

                if(d < best)
                {
                    best = d;
                    bestTarget = dmg;
                }
            }

            return bestTarget;
        }
    
    }



    
}