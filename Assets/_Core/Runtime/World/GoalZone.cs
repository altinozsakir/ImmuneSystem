using UnityEngine;
using Core.Meta;


namespace Core.World
{
    [RequireComponent(typeof(Collider))]
    public class GoalZone : MonoBehaviour
    {
        public SepsisMeter sepsis;
        public int sepsisPerEnemy = 5;


        void Awake()
        {
            var col = GetComponent<Collider>(); col.isTrigger = true;
            if (!sepsis) sepsis = FindAnyObjectByType<SepsisMeter>();
        }


        void OnTriggerEnter(Collider other)
        {
            var go = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
            if (!go || !go.activeInHierarchy) return;
            // Consider anything on Enemy layer as a breach
            if (go.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (sepsis) sepsis.Add(sepsisPerEnemy);
                Destroy(go); // remove enemy on breach
            }
        }
    }
}