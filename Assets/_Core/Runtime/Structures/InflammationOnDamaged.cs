// Assets/_Core/Runtime/Structures/InflammationOnDamaged.cs
using UnityEngine;
using Core.Meta;
using Core.Combat;

public class InflammationOnDamaged : MonoBehaviour
{
    public int perHit = 1;
    public InflammationMeter inflammation;
    public Health hp;
    void Awake(){ if (!hp) hp = GetComponent<Health>(); if (!inflammation) inflammation = FindAnyObjectByType<InflammationMeter>(); }
    void OnEnable(){ if (hp) hp.onDamaged.AddListener(OnDamaged); } // ensure Health exposes onDamaged(int dmg)
    void OnDisable(){ if (hp) hp.onDamaged.RemoveListener(OnDamaged); }
    void OnDamaged(float dmg){ if (inflammation) inflammation.Add(perHit); }
}
