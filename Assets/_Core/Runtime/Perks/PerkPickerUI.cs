using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Perks;

public static class PerkPickerLocator
{
    public static PerkPickerUI FindAny()
    {
        // Finds even inactive objects in the scene
        var all = Resources.FindObjectsOfTypeAll<PerkPickerUI>();
        return (all != null && all.Length > 0) ? all[0] : null;
    }
}

public class PerkPickerUI : MonoBehaviour
{
    public static PerkPickerUI Instance { get; private set; }


    [Header("Refs")] public PerkManager manager;
    [Header("Panels")] public Button pickA; public Button pickB; public Button rerollBtn; public TextMeshProUGUI rerollLabel;
    public TextMeshProUGUI titleA, descA, costA, titleB, descB, costB;


    (PerkData, int) _a, _b; int _qualityBias = 0; bool _open;


    void Awake()
    {
        Instance = this;
        if (!manager) manager = FindAnyObjectByType<PerkManager>();
        gameObject.SetActive(false);
    }


    public static void TryOpen(PerkManager m)
    {
        if (!Instance)
            Instance = PerkPickerLocator.FindAny();

        if (!Instance)
        {
            Debug.LogWarning("[PerkPickerUI] No instance found in scene. Add the PerkPicker panel.");
            return;
        }

        Instance.manager = m;
        Instance.Open();
    }


    void Open()
    {
        _open = true; gameObject.SetActive(true);
        // Quality bias = how many rerolls currently available (cap 2)
        _qualityBias = Mathf.Clamp(manager.rerollCharges, 0, 2);
        RollChoices();
        UpdateUI();
    }


    void Close() { _open = false; gameObject.SetActive(false); }


    void RollChoices()
    {
        var ch = manager.GenerateChoices(_qualityBias);
        _a = ch[0]; _b = ch[1];
    }


    void UpdateUI()
    {
        if (_a.Item1)
        {
            titleA.text = _a.Item1.title; descA.text = _a.Item1.description; costA.text = $"{_a.Item2} CK";
            pickA.interactable = manager.bank && manager.bank.cytokines >= _a.Item2;
            pickA.onClick.RemoveAllListeners(); pickA.onClick.AddListener(() => Pick(_a.Item1));
        }
        if (_b.Item1)
        {
            titleB.text = _b.Item1.title; descB.text = _b.Item1.description; costB.text = $"{_b.Item2} CK";
            pickB.interactable = manager.bank && manager.bank.cytokines >= _b.Item2;
            pickB.onClick.RemoveAllListeners(); pickB.onClick.AddListener(() => Pick(_b.Item1));
        }
        rerollBtn.onClick.RemoveAllListeners(); rerollBtn.onClick.AddListener(Reroll);
        rerollBtn.interactable = manager.rerollCharges > 0;
        rerollLabel.text = manager.rerollCharges.ToString();
    }


    void Reroll()
    {
        if (manager.rerollCharges <= 0) return;
        manager.ConsumeReroll();
        _qualityBias = Mathf.Clamp(manager.rerollCharges, 0, 2);
        RollChoices(); UpdateUI();
    }


    void Pick(PerkData p)
    {
        if (!manager.TryTake(p)) return; // not enough CK
        Close();
    }


    void Update() { if (_open) UpdateUI(); }
}