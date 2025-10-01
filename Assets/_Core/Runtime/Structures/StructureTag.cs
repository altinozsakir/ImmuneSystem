using UnityEngine;

namespace Core.Structures
{
    public enum StructureType { Tower, Wall, Outpost, Commander }

    // Attach to any building the enemies can attack
    public class StructureTag : MonoBehaviour
    {
        public StructureType type = StructureType.Tower;
        [Tooltip("Optional: higher = more attractive target when multiple are in range")]
        public int priority = 0;
    }
}
