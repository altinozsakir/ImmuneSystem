using UnityEngine;
using Core.Enemy.FSM;

namespace Core.Enemy
{
    [RequireComponent(typeof(EnemyInstaller))]
    public class EnemyController : MonoBehaviour
    {
        private EnemyInstaller installer;

        private void Awake()
        {
            installer = GetComponent<EnemyInstaller>();
        }

        private void Update()
        {
            var ctx = installer.BuildContext();

            installer.FSM.Tick(ref ctx);

            // apply requested transition
            if (ctx.BB.HasRequestedState)
            {
                var next = ctx.BB.RequestedState;
                ctx.BB.ClearRequest();
                installer.FSM.Change(ref ctx, next);
            }

            installer.CommitFromContext(in ctx);
        }
    }
}
