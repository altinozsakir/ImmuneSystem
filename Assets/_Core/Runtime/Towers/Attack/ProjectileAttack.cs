using UnityEngine;
using Core.Towers;
using Core.Towers.Projectiles;
using System.Runtime.ExceptionServices;


namespace Core.Towers.Attack
{
    public class ProjectileAttack : ITowerAttack
    {
        public void Tick(ref TowerContext ctx)
        {
            if(ctx.Target == null || !ctx.Target.IsAlive) return;
            if(!ctx.Cooldowns.Fire.IsReady(ctx.Now)) return;

            Vector3 muzzle = ctx.Transform.TransformPoint(ctx.Config.muzzleLocalOffset);
            Vector3 targetPos = ctx.Target.Transform.position;

            bool missed = (ctx.Config.missChance > 0f ) && (Random.value < ctx.Config.missChance);

            Vector3 aimPos = targetPos;

            if (missed)
            {
                Vector2 r = Random.insideUnitCircle * ctx.Config.missRadius;
                aimPos = targetPos + new Vector3(r.x, 0f, r.y);
            }

            if (ctx.Config.requireLineOfSight)
            {
                Vector3 dir = (aimPos - muzzle);
                float dist = dir.magnitude;
                if(dist > 0.001f)
                {
                    dir /= dist;
                    if(Physics.Raycast(muzzle,dir,dist,~0, QueryTriggerInteraction.Ignore))
                    {
                        // Blocked
                        return;
                    }
                }
            }


            var prefab = ctx.Config.projectilePrefab;
            if(prefab == null) return;

            var go = Object.Instantiate(prefab,muzzle, Quaternion.identity);
            var proj = go.GetComponent<Projectile>();

            if(proj != null)
            {
                if (missed)
                {
                    proj.InitPoint(point: aimPos,
                    damage: ctx.Config.damage,
                    speed: ctx.Config.projectileSpeed,
                    lifetime: ctx.Config.projectileLifeTime);
                }else{
                proj.Init(
                    target: ctx.Target.Transform,
                    damage: ctx.Config.damage,
                    speed: ctx.Config.projectileSpeed,
                    lifetime: ctx.Config.projectileLifeTime
                );}
            }

            ctx.Cooldowns.Fire.Arm(ctx.Now);
        }
    }
}