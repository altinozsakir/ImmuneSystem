using UnityEngine;
using Core.TimeSystem;


namespace Core.Economy
{
public class ATPWallet : MonoBehaviour
{
public BodyClockDirector clock;
[Min(0f)] public float baseIncomePerSecond = 4f;
public float Current { get; private set; }
float _atpMult = 1f;


void OnEnable()
{
if (clock)
{
clock.OnMultipliersChanged += OnMultChanged;
_atpMult = clock.Multipliers.atpIncome;
}
}
void OnDisable(){ if (clock) clock.OnMultipliersChanged -= OnMultChanged; }


void Update()
{
Current += baseIncomePerSecond * _atpMult * Time.deltaTime;
}


void OnMultChanged(PhaseMultipliers m) => _atpMult = Mathf.Max(0f, m.atpIncome);
}
}