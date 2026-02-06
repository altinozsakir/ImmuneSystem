using UnityEngine;
using Core.TimeSystem;
using Core.Meta;


namespace Core.Economy
{

    public class ATPTaxAdapter : MonoBehaviour
    {
        public ResourceBank bank;
        public BodyClockDirector clock;
        public InflammationMeter infl;


        float _pendingTax = 1f; // applies for the whole current phase


        void Awake()
        {
            if (!bank) bank = GetComponent<ResourceBank>();
            if (!clock) clock = FindAnyObjectByType<BodyClockDirector>();
            if (!infl) infl = FindAnyObjectByType<InflammationMeter>();
        }
        void OnEnable()
        {
            if (clock) clock.OnPhaseChanged += OnPhaseChanged;
            if (bank) bank.OnExternalMultiplierRequest += OnBankQuery; // add this event to ResourceBank (below)
        }
        void OnDisable()
        {
            if (clock) clock.OnPhaseChanged -= OnPhaseChanged;
            if (bank) bank.OnExternalMultiplierRequest -= OnBankQuery;
        }


        void OnPhaseChanged(BodyPhase _)
        {
            _pendingTax = infl ? infl.NextPhaseATPTaxMult : 1f; // read once at phase start
        }


        void OnBankQuery(ref float mult)
        {
            mult *= _pendingTax; // compound with existing phase multiplier
        }
    }
}