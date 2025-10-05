using UnityEngine;

namespace Core.Enemies
{
    public class EnemySpeedPhaseAdapter : MonoBehaviour
    {
        [Range(0.1f, 3f)] public float phaseSpeedMultiplier = 1f;
        public float CurrentSpeedMultiplier => phaseSpeedMultiplier;

        // Later: subscribe BodyClockDirector.OnMultipliersChanged and update:
        // phaseSpeedMultiplier = multipliers.enemySpeed;
    }
}