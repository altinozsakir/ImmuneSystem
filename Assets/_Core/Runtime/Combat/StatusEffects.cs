using UnityEngine;


namespace Core.Combat
{
public class StatusEffects : MonoBehaviour
{
[Header("Marks (bonus damage taken)")]
[Min(0)] public int markStacks = 0;
[Min(0f)] public float markBonusPerStack = 0.25f; // +25% dmg per stack
[Min(0)] public int markMaxStacks = 3;


[Header("Neutralize (reduce enemy outgoing damage)")]
[Min(0)] public int neutralizeStacks = 0;
[Range(0f,1f)] public float neutralizePerStack = 0.20f; // -20% per stack
[Min(0)] public int neutralizeMaxStacks = 2;


public float DamageTakenMultiplier()
{
return 1f + Mathf.Min(markMaxStacks, markStacks) * Mathf.Max(0f, markBonusPerStack);
}
public float OutgoingDamageMultiplier()
{
float f = 1f - Mathf.Min(neutralizeMaxStacks, neutralizeStacks) * Mathf.Clamp01(neutralizePerStack);
return Mathf.Clamp(f, 0f, 1f);
}


public void AddMarks(int stacks){ if (stacks > 0) markStacks = Mathf.Min(markMaxStacks, markStacks + stacks); }
public void AddNeutralize(int stacks){ if (stacks > 0) neutralizeStacks = Mathf.Min(neutralizeMaxStacks, neutralizeStacks + stacks); }
}
}