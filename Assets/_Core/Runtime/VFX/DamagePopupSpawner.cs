// Assets/_Core/Runtime/VFX/DamagePopupSpawner.cs
using UnityEngine;

namespace Core.VFX
{
    public class DamagePopupSpawner : MonoBehaviour
    {
        public static DamagePopupSpawner Instance { get; private set; }
        public GameObject popupPrefab;
        public Vector3 worldOffset = new Vector3(0, 0.8f, 0);

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public static void Spawn(float dmg, float hp, float maxHp, Vector3 worldPos, bool isExecute = false)
        {
            if (!Instance || !Instance.popupPrefab) return;
            var go = Instantiate(Instance.popupPrefab, worldPos + Instance.worldOffset, Quaternion.identity);
            var comp = go.GetComponent<DamagePopup>();
            if (!comp) comp = go.AddComponent<DamagePopup>();
            comp.SetDamageAndHP(dmg, hp, maxHp, isExecute);
        }

        // (kept for backward compatibility if you call the old one somewhere)
        public static void Spawn(float dmg, Vector3 worldPos, bool isExecute = false)
        {
            Spawn(dmg, 0, 0, worldPos, isExecute);
        }
    }
}
