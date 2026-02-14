using UnityEngine;

namespace Core.Towers
{
    [RequireComponent(typeof(TowerInstaller))]
    public class TowerController : MonoBehaviour
    {
        private TowerInstaller installer;

        private void Awake() => installer = GetComponent<TowerInstaller>();

        private void Update()
        {
            var ctx = installer.BuildContext();

            // Acquire/refresh target each frame (simple v1)
            ctx.Target = installer.Targeting.Acquire(in ctx);

            // Attack tick (projectile)
            installer.Attack.Tick(ref ctx);

            installer.Commit(in ctx);
        }

        private void OnDrawGizmosSelected()
        {
            var inst = GetComponent<TowerInstaller>();
            if (inst == null || inst.Config == null || !inst.Config.drawGizmos) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, inst.Config.range);
        }
    }
}
