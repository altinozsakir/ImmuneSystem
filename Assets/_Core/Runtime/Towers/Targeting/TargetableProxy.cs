using UnityEngine;

namespace Core.Towers.Targeting
{
    public class TargetableProxy : MonoBehaviour, ITargetable
    {
        public Transform Transform => transform;
        public bool IsAlive => gameObject.activeInHierarchy;
    }
    
}