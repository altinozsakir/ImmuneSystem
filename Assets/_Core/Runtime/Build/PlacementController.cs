// Assets/_Core/Runtime/Build/PlacementController.cs
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // for IsPointerOverGameObject

namespace Core.Build
{
    public class PlacementController : MonoBehaviour
    {
        [Header("Input")]
        public InputActionReference buildToggleAction; // E / West
        public InputActionReference confirmAction;     // Mouse Left / South
        public InputActionReference cancelAction;      // Esc / Start

        [Header("Placement")]
        public GameObject towerPrefab;
        public SlotType requiredSlot = SlotType.Capillary;
        public bool requireSlotMatch = true;
        public Transform towersParent; // optional

        [Header("Ghost")]
        public Material ghostMaterial;
        [Range(0f,1f)] public float ghostAlpha = 0.45f;

        [Header("Ghost Behavior")]
        public bool showGhostOnlyWhenValid = true; // <-- NEW
        public float placementYOffset = 0f;        // <-- NEW

        [Header("Raycast")]
        public string buildSlotLayerName = "BuildSlot";
        public float rayMaxDistance = 1000f;

        [Header("Economy")]
public Core.Economy.ResourceBank bank;      // <--- NEW (drag in Inspector)
public bool blockIfInsufficientATP = true;  // <--- NEW
public AudioClip denySFX;                   // optional beep when blocked

// Helper: read cost from prefab's TowerBase
int CurrentPrefabCost()
{
    if (!towerPrefab) return 0;
    var tb = towerPrefab.GetComponent<Core.Towers.TowerBase>();
    return tb ? tb.buildCostATP : 0;
}

        Camera _cam;
        bool _buildMode;
        GameObject _ghost;

        BuildNode _hoverNode;
        bool _hoverCanPlace;
        int _buildSlotMask;

        void Awake()
        {
            _cam = Camera.main;
            int layer = LayerMask.NameToLayer(buildSlotLayerName);
            if (layer < 0)
            {
                Debug.LogError($"[Placement] Layer \"{buildSlotLayerName}\" not found. Create it in Project Settings ▸ Tags and Layers.");
                _buildSlotMask = ~0;
            }
            else
            {
                _buildSlotMask = 1 << layer;
            }
        }

        void OnEnable()
        {
            if (buildToggleAction) buildToggleAction.action.performed += OnBuildToggle;
            if (cancelAction)      cancelAction.action.performed      += OnCancel;
            if (confirmAction)     confirmAction.action.performed     += OnConfirm;
        }
        void OnDisable()
        {
            if (buildToggleAction) buildToggleAction.action.performed -= OnBuildToggle;
            if (cancelAction)      cancelAction.action.performed      -= OnCancel;
            if (confirmAction)     confirmAction.action.performed     -= OnConfirm;
        }

        void OnBuildToggle(InputAction.CallbackContext _) => ToggleBuildMode();
        void OnCancel      (InputAction.CallbackContext _) { if (_buildMode) ToggleBuildMode(false); }
        void OnConfirm     (InputAction.CallbackContext _) { TryPlace(); }

        void ToggleBuildMode(bool? force = null)
        {
            bool next = force ?? !_buildMode;
            _buildMode = next;
            if (!_buildMode) { DestroyGhost(); return; }

            EnsureGhost(); // create (starts hidden)
            if (TryRaycastNode(out var node, out var pos, out var canPlace))
            {
                SetHover(node, canPlace);
                PositionGhost(pos + Vector3.up * placementYOffset);
                ShowGhost(!showGhostOnlyWhenValid || canPlace);
            }
            else
            {
                SetHover(null, false);
                ShowGhost(false); // stay hidden until valid
            }
        }

        void Update()
        {
            if (!_buildMode) return;

            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            {
                SetHover(null, false);
                ShowGhost(false);
                return;
            }

            if (TryRaycastNode(out var node, out var pos, out var canPlace))
            {
                SetHover(node, canPlace);
                EnsureGhost();
                PositionGhost(pos + Vector3.up * placementYOffset);
                ShowGhost(!showGhostOnlyWhenValid || canPlace);

                var mouse = Mouse.current;
                if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                    TryPlace();
            }
            else
            {
                SetHover(null, false);
                ShowGhost(false);
            }

            var ray = GetCursorRay();
            Debug.DrawRay(ray.origin, ray.direction * 8f, _hoverCanPlace ? Color.green : Color.red);
        }

void TryPlace()
{
    if (!_buildMode || !_hoverNode) return;

    int cost = CurrentPrefabCost();
    bool afford = !blockIfInsufficientATP || (bank && bank.SpendATP(cost));
    if (!afford)
    {
        // Feedback: treat as blocked hover + optional sound
        _hoverNode.SetHover(false, true);
        if (denySFX) AudioSource.PlayClipAtPoint(denySFX, _hoverNode.transform.position);
        return;
    }

    // Place
    var spawnPos = _hoverNode.transform.position + Vector3.up * placementYOffset;
    var real = Instantiate(towerPrefab, spawnPos, Quaternion.identity, towersParent ? towersParent : null);
    SetGhostAppearance(real, false);

    _hoverNode.occupied = true;
    _hoverNode.SetHover(false, true); // now blocked
}

        // ---- helpers ----
        Ray GetCursorRay()
        {
            if (!_cam) _cam = Camera.main;
            var mouse = Mouse.current;
            Vector2 screenPos = mouse != null ? mouse.position.ReadValue() : new Vector2(Screen.width/2, Screen.height/2);
            return _cam.ScreenPointToRay(screenPos);
        }

        bool TryRaycastNode(out BuildNode node, out Vector3 pos, out bool canPlace)
        {
            node = null; pos = Vector3.zero; canPlace = false;

            var ray = GetCursorRay();
            bool hit = Physics.Raycast(
                ray, out var hitInfo, rayMaxDistance, _buildSlotMask,
                QueryTriggerInteraction.Collide // hit trigger nodes
            );
            if (!hit) return false;

            node = hitInfo.collider.GetComponentInParent<BuildNode>();
            pos = node ? node.transform.position : hitInfo.point;
            bool slotOK = !requireSlotMatch || (node && node.slotType == requiredSlot);
            canPlace = node && !node.occupied && slotOK;
            // ... inside TryRaycastNode() after computing slotOK:
            canPlace = node && !node.occupied && slotOK && HasATPForCurrentPrefab();
            return true;
            // In TryRaycastNode(), extend canPlace to include ATP check:
            bool HasATPForCurrentPrefab()
            {
                if (!blockIfInsufficientATP) return true;
                if (!bank) return true; // allow if no bank wired yet
                return bank.atp >= CurrentPrefabCost();
            }

            
        }

        void SetHover(BuildNode node, bool canPlace)
        {
            if (_hoverNode && _hoverNode != node)
                _hoverNode.SetHover(false, false);

            _hoverNode = node;
            _hoverCanPlace = canPlace;

            if (_hoverNode)
                _hoverNode.SetHover(canPlace, true);
        }

        void EnsureGhost()
        {
            if (_ghost || !towerPrefab) return;
            _ghost = Instantiate(towerPrefab);
            _ghost.name = "[GHOST] " + towerPrefab.name;
            _ghost.layer = LayerMask.NameToLayer("Ignore Raycast");
            foreach (var col in _ghost.GetComponentsInChildren<Collider>()) col.enabled = false;
            SetGhostAppearance(_ghost, true);
            _ghost.SetActive(false); // start hidden
        }

        void DestroyGhost()
        {
            if (_ghost) Destroy(_ghost);
            _ghost = null;
            if (_hoverNode) _hoverNode.SetHover(false, false);
            _hoverNode = null; _hoverCanPlace = false;
        }

        void PositionGhost(Vector3 pos)
        {
            if (_ghost) _ghost.transform.position = pos;
        }

        void ShowGhost(bool show)
        {
            if (_ghost && _ghost.activeSelf != show) _ghost.SetActive(show);
        }

        void SetGhostAppearance(GameObject obj, bool ghost)
        {
            foreach (var r in obj.GetComponentsInChildren<Renderer>())
            {
                if (ghost)
                {
                    if (ghostMaterial)
                    {
                        r.sharedMaterial = ghostMaterial;
                        if (r.sharedMaterial.HasProperty("_BaseColor"))
                        {
                            var c = r.sharedMaterial.GetColor("_BaseColor");
                            c.a = ghostAlpha; r.sharedMaterial.SetColor("_BaseColor", c);
                        }
                    }
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    r.receiveShadows = false;
                }
            }
        }

// #if UNITY_EDITOR
//         GUIStyle _s;
//         void OnGUI()
//         {
//             if (_s == null) _s = new GUIStyle(GUI.skin.label){ fontSize = 13 };
//             GUILayout.BeginArea(new Rect(12, 12, 360, 90), GUI.skin.box);
//             GUILayout.Label($"BuildMode: {_buildMode}", _s);
//             GUILayout.Label($"HoverNode: {(_hoverNode ? _hoverNode.name : "—")}  CanPlace: {_hoverCanPlace}", _s);
//             GUILayout.Label($"LayerMask: {_buildSlotMask}", _s);
//             GUILayout.EndArea();
//         }
// #endif
    }
}
