using UnityEngine;
using Core.Combat;

namespace Core.Towers
{
    public class TowerShooter : MonoBehaviour
    {
        public TowerBase tower;
        public TowerTargetingGoal targeting;
        public GameObject projectilePrefab;
        public AudioClip shootSFX;

        void Reset()
        {
            tower = GetComponent<TowerBase>();
            targeting = GetComponent<TowerTargetingGoal>();
        }

        void Update()
        {
            if (!tower || !targeting || !projectilePrefab) return;
            if (!tower.CanFire) return;

            var target = targeting.Current;
            if (!target) return;

            var pos = tower.muzzle ? tower.muzzle.position : transform.position + transform.forward * 0.75f;
            var rot = tower.muzzle ? tower.muzzle.rotation : transform.rotation;

            var go = Instantiate(projectilePrefab, pos, rot);
            if (go.TryGetComponent<ProjectileHoming>(out var proj))
                proj.Init(target);

            if (shootSFX) AudioSource.PlayClipAtPoint(shootSFX, pos);
            tower.ResetCooldown();
        }
    }
}