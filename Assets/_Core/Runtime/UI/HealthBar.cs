using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using Core.Structures;

[DefaultExecutionOrder(10)]
public class FloatingHealthBar : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private StructureHealth target; // auto-filled to self

    [Header("Placement (LOCAL to this object)")]
    [SerializeField] private Vector3 localOffset = new Vector3(0, 2.0f, 0);

    [Header("Size & Style")]
    [SerializeField] private Vector2 sizeWorldUnits = new Vector2(1.6f, 0.18f);
    [SerializeField] private Color backColor = new Color(0, 0, 0, 0.65f);
    [SerializeField] private Color fullColor = new Color(0.2f, 0.85f, 0.2f, 0.95f);
    [SerializeField] private Color emptyColor = new Color(0.9f, 0.15f, 0.1f, 0.95f);
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private bool billboardToCamera = true;

    Camera cam;
    Canvas canvas;
    Slider slider;
    Image fillImg;
    float maxHP = 1f;

    void Awake()
    {
        if (!target) target = GetComponent<StructureHealth>();
        if (!target) { enabled = false; return; }

        // Read private maxHP once (keeps your StructureHealth unchanged)
        var f = typeof(StructureHealth).GetField("maxHP", BindingFlags.NonPublic | BindingFlags.Instance);
        maxHP = Mathf.Max(1f, f != null ? (float)f.GetValue(target) : target.hp);

        cam = Camera.main;
        BuildUIUnderRoot();
        ImmediateUpdate();
    }

    void BuildUIUnderRoot()
    {
        // Optional tidy container under the root
        var uiRoot = transform.Find("UI");
        if (!uiRoot) uiRoot = new GameObject("UI").transform;
        uiRoot.SetParent(transform, false);

        // World-space canvas AS CHILD of this object
        canvas = new GameObject("HP_Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cam;
        canvas.sortingOrder = 200;

        var crt = (RectTransform)canvas.transform;
        crt.SetParent(uiRoot, false);
        crt.sizeDelta = sizeWorldUnits * 100f;      // 100px ≈ 1 world unit
        crt.localScale = Vector3.one * 0.01f;
        crt.localPosition = localOffset;            // << local to this object
        crt.localRotation = Quaternion.identity;

        // Background
        var back = new GameObject("Back").AddComponent<Image>();
        back.transform.SetParent(canvas.transform, false);
        back.color = backColor;
        var br = back.rectTransform;
        br.anchorMin = Vector2.zero; br.anchorMax = Vector2.one;
        br.offsetMin = Vector2.zero; br.offsetMax = Vector2.zero;

        // Slider
        var sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(canvas.transform, false);
        slider = sliderGO.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.minValue = 0f; slider.maxValue = 1f; slider.value = 1f;
        var srt = slider.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(0.03f, 0.2f);
        srt.anchorMax = new Vector2(0.97f, 0.8f);
        srt.offsetMin = srt.offsetMax = Vector2.zero;

        // Fill
        var fillArea = new GameObject("FillArea").AddComponent<RectTransform>();
        fillArea.transform.SetParent(sliderGO.transform, false);
        fillArea.anchorMin = Vector2.zero; fillArea.anchorMax = Vector2.one;
        fillArea.offsetMin = fillArea.offsetMax = Vector2.zero;

        fillImg = new GameObject("Fill").AddComponent<Image>();
        fillImg.transform.SetParent(fillArea.transform, false);
        fillImg.color = fullColor;
        var fr = fillImg.rectTransform;
        fr.anchorMin = Vector2.zero; fr.anchorMax = Vector2.one;
        fr.offsetMin = fr.offsetMax = Vector2.zero;

        slider.fillRect = fr;
        slider.targetGraphic = fillImg;
    }

    void LateUpdate()
    {
        if (!target || !canvas) return;

        // Keep local offset (follows animations/scale/rig)
        var crt = (RectTransform)canvas.transform;
        crt.localPosition = localOffset;

        // Billboard in world space while still parented under the root
        if (billboardToCamera)
        {
            if (!cam) cam = Camera.main;
            if (cam) canvas.transform.rotation =
                Quaternion.LookRotation(canvas.transform.position - cam.transform.position);
        }

        // Update value & color
        float t = Mathf.Clamp01(target.hp / Mathf.Max(0.0001f, maxHP));
        slider.value = t;
        fillImg.color = Color.Lerp(emptyColor, fullColor, t);

        if (hideWhenFull) canvas.enabled = t < 0.999f && target.IsAlive;

        // Cleanup on death (child is auto-destroyed with root, but do it eagerly)
        if (!target.IsAlive || target == null) Destroy(this);
    }

    void ImmediateUpdate()
    {
        float t = Mathf.Clamp01(target.hp / Mathf.Max(0.0001f, maxHP));
        slider.value = t;
        fillImg.color = Color.Lerp(emptyColor, fullColor, t);
        if (hideWhenFull) canvas.enabled = t < 0.999f && target.IsAlive;
    }
}
