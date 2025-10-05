using UnityEngine;

namespace Core.Combat
{
    /// Aggregates slows and snares, applies to NavAgentMover.
    
public class CrowdControl : MonoBehaviour
{
    [Range(0f, 1f)] public float slowResist = 0f; // e.g., 0.2 = 20% resist
    [Range(0f, 1f)] public float slowSum = 0f;    // accumulate external slows [0..1]
    public bool snared = false;

    const float MAX_SLOW = 0.70f; // cap 70%

    public float SpeedMultiplier
    {
        get
        {
            if (snared) return 0f;
            // effective slow after resist
            float effSlow = Mathf.Clamp01(slowSum * (1f - slowResist));
            effSlow = Mathf.Min(effSlow, MAX_SLOW);
            return 1f - effSlow;
        }
    }

    public void ApplySlow(float slow01, float duration)
    {
        // simple demo: latch slow for duration
        slowSum = Mathf.Clamp01(slowSum + slow01);
        CancelInvoke(nameof(ClearSlow));
        Invoke(nameof(ClearSlow), duration);
    }

    public void ApplySnare(float duration)
    {
        snared = true;
        CancelInvoke(nameof(ClearSnare));
        Invoke(nameof(ClearSnare), duration);
    }

    void ClearSlow()  => slowSum = 0f;
    void ClearSnare() => snared = false;
}
}
