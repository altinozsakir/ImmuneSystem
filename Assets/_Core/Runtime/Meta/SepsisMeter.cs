using System;
using UnityEngine;


namespace Core.Meta
{
public class SepsisMeter : MonoBehaviour
{
public MeterConfig config;
[Min(0)] public int value; // 0..max


public event Action<int> OnChanged;
public event Action OnDefeat;


public int Max => config ? config.maxValue : 100;
public float Normalized => Mathf.Clamp01((float)value / Mathf.Max(1, Max));


public void Add(int delta)
{
int nv = Mathf.Clamp(value + delta, 0, Max);
if (nv == value) return;
value = nv; OnChanged?.Invoke(value);
if (value >= Max) OnDefeat?.Invoke();
}
public void Set(int v){ v = Mathf.Clamp(v, 0, Max); if (v == value) return; value = v; OnChanged?.Invoke(value); if (value >= Max) OnDefeat?.Invoke(); }
}
}