using UnityEngine;
using Core.Economy;   // ResourceBank
using Core.Build;    // BuildNode, SlotType/SlotMask

[System.Serializable]
public class TowerOption
{
    public string id = "Tower";
    public GameObject prefab;
    public GameObject ghost;
    [Min(0)] public int costATP = 25;
    public SlotMask allowedSlots = SlotMask.Any;
}

public class PlacementControllerMulti : MonoBehaviour
{
    [Header("Input")]
    public KeyCode toggleKey = KeyCode.E;
    public KeyCode placeKey = KeyCode.Mouse0;
    public KeyCode cancelKey = KeyCode.Escape;
    public KeyCode rotateKey = KeyCode.R;
    public KeyCode prevKey = KeyCode.Q;
    public KeyCode nextKey = KeyCode.E; // change if you use E to toggle

    [Header("Scene Refs")]
    public Camera cam;                 // auto-fills with Camera.main if null
    public ResourceBank bank;          // auto-find if null
    public LayerMask buildSlotMask;    // set to BuildSlot (auto-set if 0)
    public float rayMax = 500f;

    [Header("Behavior")]
    public float placementYOffset = 0.05f;
    public bool showGhostOnlyWhenValid = true;
    public bool blockIfInsufficientATP = true;

    [Header("Catalog")]
    public TowerOption[] towers;

    [Header("Optional HUD")]
    public TMPro.TextMeshProUGUI hotbarLabel;

    // runtime
    int _sel = 0;
    bool _active;
    GameObject _ghost;
    Quaternion _ghostRot = Quaternion.identity;
    BuildNode _hoverNode;
    BuildNode _lastHoverNode;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!bank) bank = FindAnyObjectByType<ResourceBank>();
        if (buildSlotMask == 0) buildSlotMask = 1 << LayerMask.NameToLayer("BuildSlot");
        ClampSel();
    }

    void Update()
    {
        // Toggle build mode
        if (Input.GetKeyDown(toggleKey)) SetActive(!_active);

        // Select by number keys (1..9)
        for (int i = 0; i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) { SetSelection(i); if (!_active) SetActive(true); }

        // Cycle by mouse wheel
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f) Cycle(scroll > 0 ? -1 : 1); // up = previous

        // Cycle by keys
        if (Input.GetKeyDown(prevKey)) Cycle(-1);
        if (Input.GetKeyDown(nextKey)) Cycle(+1);

        if (!_active)
        {
            UpdateHotbarLabel();
            return;
        }

        // Cancel
        if (Input.GetKeyDown(cancelKey)) { SetActive(false); return; }

        // Rotate ghost
        if (Input.GetKeyDown(rotateKey))
            _ghostRot = Quaternion.Euler(0f, _ghostRot.eulerAngles.y + 90f, 0f);

        // Hover + ghost update
        UpdateHover();

        // Place
        if (Input.GetKeyDown(placeKey))
            TryPlace();

        UpdateHotbarLabel();
    }

    void SetActive(bool on)
    {
        _active = on;
        if (_active)
        {
            EnsureGhost();
        }
        else
        {
            DestroyGhost();
            ClearHover();
        }
    }

    void SetSelection(int index)
    {
        if (towers == null || towers.Length == 0) return;
        _sel = Mathf.Clamp(index, 0, towers.Length - 1);
        // swap ghost to selected type
        DestroyGhost();
        EnsureGhost();
    }

    void Cycle(int dir)
    {
        if (towers == null || towers.Length == 0) return;
        _sel = ((_sel + dir) % towers.Length + towers.Length) % towers.Length;
        DestroyGhost();
        EnsureGhost();
    }

    void ClampSel()
    {
        if (towers == null || towers.Length == 0) _sel = 0;
        else _sel = Mathf.Clamp(_sel, 0, towers.Length - 1);
    }

    TowerOption Sel => (towers != null && towers.Length > 0) ? towers[_sel] : null;

    void EnsureGhost()
    {
        var opt = Sel;
        if (opt == null) return;
        if (!_ghost && opt.ghost)
        {
            _ghost = Instantiate(opt.ghost);
            PrepGhost(_ghost);
        }
    }

    void DestroyGhost()
    {
        if (_ghost) Destroy(_ghost);
        _ghost = null;
    }

    static void PrepGhost(GameObject g)
    {
        // make sure the ghost has no physics/logic
        foreach (var c in g.GetComponentsInChildren<Collider>()) c.enabled = false;
        foreach (var rb in g.GetComponentsInChildren<Rigidbody>()) rb.isKinematic = true;
    }

    void UpdateHover()
    {
        // clear previous node highlight if any
        if (_lastHoverNode && _lastHoverNode != _hoverNode)
            _lastHoverNode.SetHover(false, false);

        _lastHoverNode = _hoverNode;
        _hoverNode = null;

        // Raycast only to BuildSlot layer
        if (cam && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, rayMax, buildSlotMask, QueryTriggerInteraction.Collide))
            _hoverNode = hit.collider.GetComponentInParent<BuildNode>();

        var opt = Sel;
        bool valid = false;

        if (_ghost)
        {
            // compute validity
            bool slotOk = SlotIsAllowed(opt, _hoverNode);
            bool canAfford = !blockIfInsufficientATP || (bank && bank.atp >= (opt?.costATP ?? 0));
            valid = _hoverNode && slotOk && canAfford && NodeFree(_hoverNode);

            // show/hide ghost
            if (showGhostOnlyWhenValid) _ghost.SetActive(valid);
            else _ghost.SetActive(true);

            // position/rotation
            if (_hoverNode)
            {
                var pos = _hoverNode.GetPlacePosition(placementYOffset);
                var rot = _hoverNode.GetPlaceRotation(_ghostRot);
                _ghost.transform.SetPositionAndRotation(pos, rot);
            }

            // tint (if ghost has a renderer)
            var rend = _ghost.GetComponentInChildren<Renderer>();
            if (rend)
            {
                var col = valid ? new Color(0.2f, 1f, 0.4f, 0.75f) : new Color(1f, 0.35f, 0.35f, 0.75f);
                var mpb = new MaterialPropertyBlock();
                rend.GetPropertyBlock(mpb);
                mpb.SetColor("_BaseColor", col);
                if (mpb.HasVector("_Color")) mpb.SetColor("_Color", col); // legacy shaders
                rend.SetPropertyBlock(mpb);
            }
        }

        // drive node hover visual
        if (_hoverNode) _hoverNode.SetHover(valid, true);
    }

    void ClearHover()
    {
        if (_hoverNode) _hoverNode.SetHover(false, false);
        if (_lastHoverNode) _lastHoverNode.SetHover(false, false);
        _hoverNode = _lastHoverNode = null;
    }

    static bool SlotIsAllowed(TowerOption opt, BuildNode node)
    {
        if (opt == null || node == null) return false;
        if (opt.allowedSlots == SlotMask.Any) return true;
        var mask = SlotMaskUtil.FromType(node.slotType);
        return (opt.allowedSlots & mask) != 0;
    }

    static bool NodeFree(BuildNode node) => node && !node.IsOccupied;

    void TryPlace()
    {
        var opt = Sel;
        if (opt == null || !opt.prefab || !_hoverNode) return;
        if (!SlotIsAllowed(opt, _hoverNode)) return;
        if (!NodeFree(_hoverNode)) return;

        if (blockIfInsufficientATP && bank && bank.atp < opt.costATP)
            return;

        // Spend ATP
        if (bank && opt.costATP > 0) bank.SpendATP(opt.costATP);

        // Spawn
        var pos = _hoverNode.GetPlacePosition(placementYOffset);
        var rot = _hoverNode.GetPlaceRotation(_ghostRot);
        var parent = _hoverNode.GetParent(transform);
        var placed = Instantiate(opt.prefab, pos, rot, parent);

        // Occupy node (auto-releases when placed object is destroyed)
        if (!_hoverNode.TryOccupy(placed))
        {
            // race/edge—refund and destroy
            if (bank && opt.costATP > 0) bank.GainATP(opt.costATP);
            Destroy(placed);
            return;
        }
    }

    void UpdateHotbarLabel()
    {
        if (!hotbarLabel) return;
        if (towers == null || towers.Length == 0) { hotbarLabel.text = $"Press {toggleKey} to Build"; return; }

        var t = Sel;
        if (!_active)
            hotbarLabel.text = $"Press {toggleKey} to Build  (1..{towers.Length} to select)";
        else
            hotbarLabel.text = $"[{_sel + 1}] {t.id} — {t.costATP} ATP";
    }
}
