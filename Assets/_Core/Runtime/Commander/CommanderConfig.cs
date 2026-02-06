using UnityEngine;

namespace  Core.Commander
{
    [CreateAssetMenu(menuName ="ImmuneTD/Commander/CommanderConfig")]
    public class CommanderConfig: ScriptableObject
    {

        [Header("Locomotion")] 
        public float moveSpeed = 6f;

        [Header("Dash")]
        public float dashSpeed = 14f;
        public float dashDuration = 0.18f;
        public float dashCooldown = 2.0f;
    }
}