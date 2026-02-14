using UnityEngine;

namespace Core.Towers
{
    [CreateAssetMenu(menuName ="ImmuneTD/Towers/TowerConfig")]
    public class TowerConfig : ScriptableObject
    {
        [Header("Core")]
        public string towerName = "Basic Tower";
        public float range=6f;

        public float fireCooldown = 0.7f;
        public float damage=10f;

        public GameObject projectilePrefab;


        public float projectileSpeed = 12f;
        public float projectileLifeTime=3f;

        public LayerMask targetMask;
        public bool requireLineOfSight = false;

        public Vector3 muzzleLocalOffset = new Vector3(0f,1.0f,0f);

        public bool drawGizmos = true;

        [Header("Accuracy")]
        [Range(0f, 1f)]
        public float missChance = 0.0f;   // 0 = never miss, 1 = always miss

        [Tooltip("When a shot misses, aim at an offset point around the target (world units).")]
        [Min(0f)]
        public float missRadius = 0.75f;

    }
}