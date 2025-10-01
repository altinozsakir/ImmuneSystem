// Assets/_Core/Runtime/Structures/SepsisOnDeath.cs
using UnityEngine;
using Core.Meta;  // SepsisMeter
using Core.Combat;

public class SepsisOnDeath : MonoBehaviour
{
    public int add = 5;
    public SepsisMeter sepsis;
    public Health hp;
    void Awake(){ if (!hp) hp = GetComponent<Health>(); if (!sepsis) sepsis = FindAnyObjectByType<SepsisMeter>(); }
    void OnEnable(){ if (hp) hp.onDeath.AddListener(OnDeath); }
    void OnDisable(){ if (hp) hp.onDeath.RemoveListener(OnDeath); }
    void OnDeath(){ if (sepsis) sepsis.Add(add); }
}
