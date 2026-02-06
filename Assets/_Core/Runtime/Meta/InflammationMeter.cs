using System;
using UnityEngine;


namespace Core.Meta
{
    public class InflammationMeter : MonoBehaviour
    {
        public MeterConfig config;
        [Min(0)] public int value; // 0..max


        public event Action<int> OnChanged;


        public int Max => config ? config.maxValue : 100;
        public float Normalized => Mathf.Clamp01((float)value / Mathf.Max(1, Max));


        public int Level => Mathf.RoundToInt(Normalized * 10f); // 0..10 convenience


        public float DamageBonusMult => 1f + Level * (config ? config.inflDamageBonusPerLevel : 0f);
        public float ExecuteBonus => Level * (config ? config.inflExecuteBonusPerLevel : 0f); // add to threshold


        public float NextPhaseATPTaxMult
        {
            get
            {
                int baseline = config ? config.inflBaselineLevel : 4;
                int over = Mathf.Max(0, Level - baseline);
                int steps = over / 2; // per +2 above baseline
                return Mathf.Clamp01(1f - steps * 0.10f);
            }
        }


        public void Add(int delta)
        {
            int nv = Mathf.Clamp(value + delta, 0, Max); if (nv == value) return; value = nv; OnChanged?.Invoke(value);
        }
        public void Set(int v) { v = Mathf.Clamp(v, 0, Max); if (v == value) return; value = v; OnChanged?.Invoke(value); }
    }
}