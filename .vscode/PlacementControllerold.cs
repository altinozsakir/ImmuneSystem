// // Assets/_Core/Runtime/Build/PlacementController.cs
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.EventSystems; // for UI blocking

// namespace Core.Build
// {
//     public class PlacementController : MonoBehaviour
//     {
//         [Header("Input")]
//         public InputActionReference buildToggleAction; // e.g., E / West
//         public InputActionReference confirmAction;     // e.g., Mouse Left / South
//         public InputActionReference cancelAction;      // e.g., Esc / Start

//         [Header("Placement")]
//         public GameObject towerPrefab;
//         public SlotType requiredSlot = SlotType.Capillary;
//         public bool requireSlotMatch = true; // turn OFF to place on any slot
//         public Transform towersParent;       // optional container

//         [Header("Ghost")]
//         public Material ghostMaterial;
//         [Range(0f,1f)] public float ghostAlpha = 0.45f;

//         [Header("Raycast")]
//         public string buildSlotLayerName = "BuildSlot";
//         public float rayMaxDistance = 1000f;

//         Camera _cam;
//         bool _buildMode;
//         GameObject _ghost;

//         BuildNode _hoverNode;
//         bool _hoverCanPlace;
//         int _buildSlotMask;

//         void Awake()
//         {
//             _cam = Camera.main;
//             // SAFER than LayerMask.GetMask: this fails fast if layer doesn't exist
//             int layer = LayerMask.NameToLayer(buildSlotLayerName);
//             if (layer < 0)
//             {
//                 Debug.LogError($"[Placement] Layer \"{buildSlotLayerName}\" does not exist. Create it in Project Settings ▸ Tags and Layers.");
//                 _buildSlotMask = ~0; // fallback so you can still raycast
//             }
//             else
//             {
//                 _buildSlotMask = 1 << layer;
//             }
//         }

//         void OnEnable()
//         {
//             if (buildToggleAction) buildToggleAction.action.performed += OnBuildToggle;
//             if (cancelAction) cancelAction.action.performed += OnCancel;
//             if (confirmAction) confirmAction.action.performed += OnConfirm;
//         }
//         void OnDisable()
//         {
//             if (buildToggleAction) buildToggleAction.action.performed -= OnBuildToggle;
//             if (cancelAction) cancelAction.action.performed -= OnCancel;
//             if (confirmAction) confirmAction.action.performed -= OnConfirm;
//         }

//         void OnBuildToggle(InputAction.CallbackContext _) => ToggleBuildMode();
//         void OnCancel   (InputAction.CallbackContext _) { if (_buildMode) ToggleBuildMode(false); }
//         void OnConfirm  (InputAction.CallbackContext _) { TryPlace(); }

//         void ToggleBuildMode(bool? force = null)
//         {
//             bool next = force ?? !_buildMode;
//             _buildMode = next;
//             if (!_buildMode) DestroyGhost();
//             else EnsureGhost();
//         }

//         void Update()
//         {
//             if (!_buildMode) return;

//             if (!_cam) _cam = Camera.main;

//             // ignore when pointer is over UI
//             if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
//             {
//                 SetHover(null, false);
//                 return;
//             }

//             // Screen ray → BuildSlot layer, ensure we hit triggers
//             var mouse = Mouse.current;
//             Vector2 screenPos = mouse != null ? mouse.position.ReadValue() : new Vector2(Screen.width/2, Screen.height/2);
//             Ray ray = _cam.ScreenPointToRay(screenPos);

//             bool hitSomething = Physics.Raycast(
//                 ray,
//                 out var hit,
//                 rayMaxDistance,
//                 _buildSlotMask,
//                 QueryTriggerInteraction.Collide // <- critical if BuildNode uses trigger colliders
//             );

//             if (hitSomething)
//             {
//                 var node = hit.collider.GetComponentInParent<BuildNode>();
//                 bool canPlace = node
//                                 && !node.occupied
//                                 && (!requireSlotMatch || node.slotType == requiredSlot);

//                 SetHover(node, canPlace);
//                 EnsureGhost();
//                 PositionGhost(node ? node.transform.position : hit.point);

//                 // Mouse fallback click
//                 if (mouse != null && mouse.leftButton.wasPressedThisFrame)
//                     TryPlace();
//             }
//             else
//             {
//                 SetHover(null, false);
//                 PositionGhost(ray.GetPoint(10f));
//             }

//             // Visualize the ray in Scene view
//             Debug.DrawRay(ray.origin, ray.direction * 8f, _hoverCanPlace ? Color.green : Color.red);
//         }

//         void TryPlace()
//         {
//             if (!_buildMode || !_hoverNode || !_hoverCanPlace) return;
//             // Place
//             var spawnPos = _hoverNode.transform.position;
//             var real = Instantiate(towerPrefab, spawnPos, Quaternion.identity, towersParent ? towersParent : null);
//             // Restore materials/shadows on the real one
//             SetGhostAppearance(real, false);

//             _hoverNode.occupied = true;
//             SetHover(_hoverNode, false); // update color to blocked
//         }

//         void SetHover(BuildNode node, bool canPlace)
//         {
//             if (_hoverNode && _hoverNode != node)
//                 _hoverNode.SetHover(false, false); // clear previous

//             _hoverNode = node;
//             _hoverCanPlace = canPlace;

//             if (_hoverNode)
//                 _hoverNode.SetHover(canPlace, true);
//         }

//         void EnsureGhost()
//         {
//             if (_ghost || !towerPrefab) return;
//             _ghost = Instantiate(towerPrefab);
//             _ghost.name = "[GHOST] " + towerPrefab.name;
//             // Ghost should never block raycasts; put on IgnoreRaycast
//             _ghost.layer = LayerMask.NameToLayer("Ignore Raycast");
//             // Disable any colliders on the ghost
//             foreach (var col in _ghost.GetComponentsInChildren<Collider>()) col.enabled = false;
//             SetGhostAppearance(_ghost, true);
//         }

//         void DestroyGhost()
//         {
//             if (_ghost) Destroy(_ghost);
//             _ghost = null;
//             if (_hoverNode) _hoverNode.SetHover(false, false);
//             _hoverNode = null;
//             _hoverCanPlace = false;
//         }

//         void PositionGhost(Vector3 pos)
//         {
//             if (_ghost) _ghost.transform.position = pos;
//         }

//         void SetGhostAppearance(GameObject obj, bool ghost)
//         {
//             foreach (var r in obj.GetComponentsInChildren<Renderer>())
//             {
//                 if (ghost)
//                 {
//                     if (ghostMaterial)
//                     {
//                         r.sharedMaterial = ghostMaterial;
//                         if (r.sharedMaterial.HasProperty("_BaseColor"))
//                         {
//                             var c = r.sharedMaterial.GetColor("_BaseColor");
//                             c.a = ghostAlpha;
//                             r.sharedMaterial.SetColor("_BaseColor", c);
//                         }
//                     }
//                     r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//                     r.receiveShadows = false;
//                 }
//                 else
//                 {
//                     // leave prefab's original materials; shadows default
//                 }
//             }
//         }

// // #if UNITY_EDITOR
// //         // Tiny on-screen debug so you can verify state quickly
// //         GUIStyle _s;
// //         void OnGUI()
// //         {
// //             if (_s == null) _s = new GUIStyle(GUI.skin.label){ fontSize = 13 };
// //             GUILayout.BeginArea(new Rect(12, 12, 360, 90), GUI.skin.box);
// //             GUILayout.Label($"BuildMode: {_buildMode}", _s);
// //             GUILayout.Label($"HoverNode: {(_hoverNode ? _hoverNode.name : "—")}  CanPlace: {_hoverCanPlace}", _s);
// //             GUILayout.Label($"LayerMask: {_buildSlotMask}", _s);
// //             GUILayout.EndArea();
// //         }
// // #endif

//     }
// }
