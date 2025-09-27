using UnityEngine;
using Core.Meta;


namespace Core.Combat
{
/// Global read-only modifiers driven by Inflammation; projectiles/health can consult.
public static class GlobalCombatModifiers
{
public static float DamageMult { get; private set; } = 1f;
public static float ExecuteBonus { get; private set; } = 0f; // absolute


public static void Bind(InflammationMeter infl)
{
if (!infl) return;
void Apply(int _)
{
DamageMult = infl.DamageBonusMult;
ExecuteBonus = infl.ExecuteBonus; // e.g., +0.03 adds 3% to thresholds
}
infl.OnChanged += Apply; Apply(infl.value);
}
}
}