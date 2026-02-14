using UnityEngine;
using Core.Towers;
using Core.Towers.Targeting;

namespace Core.Towsers.Targeting
{
    public class NearestTargeting: ITowerTargeting
    {
        public ITargetable Acquire(in TowerContext ctx)
        {
            float r = ctx.Config.range;
            var hits = Physics.OverlapSphere(ctx.Transform.position,r,ctx.Config.targetMask, QueryTriggerInteraction.Ignore);

            float best = float.PositiveInfinity;
            ITargetable bestT = null;

            for(int i=0; i < hits.Length; i++)
            {
                var t = hits[i].GetComponentInParent<ITargetable>();
                if(t== null || !t.IsAlive) continue;

                float d = (t.Transform.position - ctx.Transform.position).sqrMagnitude;

                if (d < best)
                {
                    best = d;
                    bestT = t;
                }
            }
            return bestT;
        }
    }
}