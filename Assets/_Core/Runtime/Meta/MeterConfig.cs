using UnityEngine;


namespace Core.Meta
{
[CreateAssetMenu(menuName = "ImmuneTD/MeterConfig")]
public class MeterConfig : ScriptableObject
{
[Min(1)] public int maxValue = 100; // 0..max
public Color lowColor = Color.green;
public Color midColor = Color.yellow;
public Color highColor = Color.red;


[Header("Inflammation tuning (optional)")]
[Tooltip("Baseline level for ATP tax computation (e.g., 4). Each +2 over baseline costs 10% next phase.")]
public int inflBaselineLevel = 4;
public float inflDamageBonusPerLevel = 0.05f; // +5% dmg per level
public float inflExecuteBonusPerLevel = 0.01f; // +1% absolute execute per level
}
}