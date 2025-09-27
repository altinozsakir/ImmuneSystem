using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.TimeSystem;


namespace Core.UI
{
    public class ClockRingHUD : MonoBehaviour
    {
        [Header("Refs")]
        public BodyClockDirector clock;
        public Image ringBack; // static background ring
        public Image ringFill; // phase progress (radial 360)
        public Image planningWedge; // ACTIVE planning window wedge (radial 360)
        public Image cursor; // small dot rotating around the circle
        public Text phaseLabel; // or TMP_Text


        [Header("Pips (upcoming planning starts)")]
        public RectTransform pipContainer;
        public Image pipPrefab;
        public Color planningPipColor = new Color(1f, 0.95f, 0.6f, 1f);


        float _cycleLen; // total duration of all phases
        List<float> _phaseStart; // cumulative seconds at phase starts
        readonly List<Image> _pips = new();


        void Start()
        {
            BuildTimeline();
            BuildPips();
            if (clock)
            {
                clock.OnPhaseChanged += _ => UpdateImmediate();
                clock.OnPlanningWindow += _ => UpdateImmediate();
            }
            UpdateImmediate();
        }


        void Update() { UpdateImmediate(); }


        void BuildTimeline()
        {
            _cycleLen = 0f; _phaseStart = new List<float>();
            if (!clock || !clock.config) return;
            var defs = clock.config.phases;
            for (int i = 0; i < defs.Length; i++)
            {
                _phaseStart.Add(_cycleLen);
                _cycleLen += Mathf.Max(0.001f, defs[i].durationSec);
            }
        }
        void BuildPips()
        {
            if (!pipContainer || !pipPrefab || !clock || !clock.config) return;
            foreach (var p in _pips) if (p) Destroy(p.gameObject);
            _pips.Clear();
            var defs = clock.config.phases;
            for (int i = 0; i < defs.Length; i++)
            {
                if (!defs[i].startPlanningOnEnter) continue;
                float t = _phaseStart[i] / _cycleLen; // at phase start
                var pip = Instantiate(pipPrefab, pipContainer);
                pip.color = planningPipColor;
                pip.rectTransform.localEulerAngles = new Vector3(0, 0, -t * 360f);
                _pips.Add(pip);
            }
        }


        void UpdateImmediate()
        {
            if (!clock) return;


            // Phase color & progress
            if (ringFill)
            {
                ringFill.color = clock.PhaseColor;
                ringFill.fillMethod = Image.FillMethod.Radial360;
                ringFill.fillOrigin = (int)Image.Origin360.Top;
                ringFill.fillAmount = Mathf.Clamp01(clock.PhaseProgress);
            }


            // Cursor: full-day normalized rotation
            if (cursor)
            {
                float dayT = DayNormalized();
                cursor.rectTransform.localEulerAngles = new Vector3(0, 0, -dayT * 360f);
            }


            if (phaseLabel) phaseLabel.text = clock.CurrentPhase.ToString();


            // Active planning wedge (unscaled-time driven)
            if (planningWedge)
            {
                bool active = clock.IsPlanningActive && clock.PlanningWindowDurationUnscaled > 0f;
                planningWedge.gameObject.SetActive(active);
                if (active)
                {
                    planningWedge.color = Color.Lerp(Color.white, clock.PhaseColor, 0.35f);
                    planningWedge.fillMethod = Image.FillMethod.Radial360;
                    planningWedge.fillOrigin = (int)Image.Origin360.Top;
                    float p = Mathf.Clamp01(clock.PlanningWindowElapsedUnscaled / Mathf.Max(0.0001f, clock.PlanningWindowDurationUnscaled));
                    planningWedge.fillAmount = p; // from 0→1 over the window
                }
            }
        }


        float DayNormalized()
        {
            if (!clock || !clock.config || _cycleLen <= 0f) return 0f;
            int idx = clock.config.IndexOf(clock.CurrentPhase); if (idx < 0) idx = 0;
            float start = _phaseStart[idx];
            float t = (start + clock.PhaseProgress * Mathf.Max(0.001f, clock.PhaseDuration)) / _cycleLen;
            return Mathf.Repeat(t, 1f);
        }
    }
}