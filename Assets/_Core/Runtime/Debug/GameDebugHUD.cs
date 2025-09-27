using UnityEngine;
using Core.TimeSystem;
using Core.Economy;
using Core.Meta;
using Core.Combat;


public class GameDebugHUD : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.F1;
    public bool visible = true;


    [Header("Refs (optional, auto-find if empty)")]
    public BodyClockDirector clock;
    public ResourceBank bank;
    public InflammationMeter inflammation;
    public SepsisMeter sepsis;


    int _enemyCount; float _enemyScanTimer;


    void Awake()
    {
        if (!clock) clock = FindAnyObjectByType<BodyClockDirector>();
        if (!bank) bank = FindAnyObjectByType<ResourceBank>();
        if (!inflammation) inflammation = FindAnyObjectByType<InflammationMeter>();
        if (!sepsis) sepsis = FindAnyObjectByType<SepsisMeter>();
    }


    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) visible = !visible;


        _enemyScanTimer -= Time.unscaledDeltaTime;
        if (_enemyScanTimer <= 0f)
        {
            _enemyScanTimer = 0.5f; // light scan twice a second
#if UNITY_2022_2_OR_NEWER
            var enemies = FindObjectsByType<Core.Combat.Health>(FindObjectsSortMode.None);
#else
var enemies = FindObjectsOfType<Core.Combat.Health>();
#endif
            int c = 0;
            foreach (var h in enemies) if (h && h.gameObject.activeInHierarchy) c++;
            _enemyCount = c;
        }
    }


    GUIStyle _lh, _vh; Rect _box = new Rect(12, 12, 380, 380);
    void OnGUI()
    {
        if (!visible) return;
        if (_lh == null)
        {
            _lh = new GUIStyle(GUI.skin.label) { fontSize = 12 };
            _vh = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        }


        GUILayout.BeginArea(_box, GUI.skin.box);
        GUILayout.Label("DEBUG HUD", _vh);


        if (clock)
        {
            GUILayout.Label($"Phase: {clock.CurrentPhase} prog: {clock.PhaseProgress * 100f:0}% timeScale: {Time.timeScale:0.00}", _lh);
            GUILayout.Label($"Planning: {(clock.IsPlanningActive ? "ACTIVE" : "—")} window: {clock.PlanningWindowElapsedUnscaled:0.0}/{clock.PlanningWindowDurationUnscaled:0.0}s", _lh);
        }


        if (bank)
        {
            GUILayout.Label($"ATP: {bank.atp:0} tick: {bank.atpPerTick:0} / {bank.tickIntervalSec:0}s (phase×tax applied)", _lh);
        }


        if (inflammation)
        {
            int lvl = inflammation.Level; float tax = 1f; if (inflammation) tax = inflammation.NextPhaseATPTaxMult;
            GUILayout.Label($"Inflammation: {inflammation.value}/{inflammation.Max} lvl {lvl} dmg×{GlobalCombatModifiers.DamageMult:0.00} exec+{GlobalCombatModifiers.ExecuteBonus * 100f:0.#}% nextPhase ATP×{tax:0.00}", _lh);
        }


        if (sepsis)
        {
            GUILayout.Label($"Sepsis: {sepsis.value}/{sepsis.Max} ({sepsis.Normalized * 100f:0.#}%)", _lh);
        }


        GUILayout.Label($"Enemies alive: {_enemyCount}", _lh);
        GUILayout.EndArea();
    }
}