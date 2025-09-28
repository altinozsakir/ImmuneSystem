using UnityEngine;
using Core.Meta;
using Core.Combat;
public class _InflBind : MonoBehaviour { public InflammationMeter infl; void Start() { if (!infl) infl = FindAnyObjectByType<InflammationMeter>(); GlobalCombatModifiers.Bind(infl); } }