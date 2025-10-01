using UnityEngine;

namespace Core.Build
{
    public enum SlotType { Capillary, Lymph, Barrier }

    /// A single placement slot in the world. Lives on layer "BuildSlot".
    /// Handles: allowed slot type, hover visuals (optional), occupancy & parent.
    [RequireComponent(typeof(Collider))]
    public class BuildNode : MonoBehaviour
    {
        [Header("Identity")]
        public SlotType slotType = SlotType.Capillary;

        [Header("Parenting")]
        [Tooltip("If set, placed towers will be parented here; otherwise this transform.")]
        public Transform snapParent;

        [Header("Use Rules")]
        [Tooltip("If true, this node can only hold one tower at a time.")]
        public bool singleUse = true;

        [Header("Visuals (optional)")]
        [Tooltip("Optional: a Renderer to tint for hover/valid/blocked states.")]
        public Renderer ringRenderer;
        public Color idle = new(0.25f, 0.25f, 0.25f, 1f);
        public Color canPlace = new(0.2f, 1f, 0.4f, 1f);
        public Color blocked = new(1f, 0.35f, 0.35f, 1f);

        // Runtime
        public bool IsOccupied { get; private set; }
        public GameObject Occupant { get; private set; }

        Collider _col;
        MaterialPropertyBlock _mpb;

        void Reset()
        {
            // Make sure this is a trigger on the BuildSlot layer
            gameObject.layer = LayerMask.NameToLayer("BuildSlot");
            var c = GetComponent<Collider>();
            c.isTrigger = true;
        }

        void Awake()
        {
            _col = GetComponent<Collider>();
            _col.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("BuildSlot");
            _mpb = new MaterialPropertyBlock();
            SetTint(idle);
        }

        // --- API used by PlacementControllerMulti ---

        public Vector3 GetPlacePosition(float yOffset)
        {
            var p = transform.position;
            p.y += yOffset;
            return p;
        }

        public Quaternion GetPlaceRotation(Quaternion desired)
        {
            // Lock to node's Y so towers align consistently; keep desired yaw offset
            var yaw = transform.rotation.eulerAngles.y + desired.eulerAngles.y;
            return Quaternion.Euler(0f, yaw, 0f);
        }

        public Transform GetParent(Transform fallback) => snapParent ? snapParent : transform;

        /// Called while hovering; controller provides validity.
        public void SetHover(bool canPlaceHere, bool hovering)
        {
            if (!ringRenderer) return;
            if (!hovering) { SetTint(idle); return; }
            SetTint(canPlaceHere ? canPlace : blocked);
        }

        /// Try to occupy this node with a placed object. Returns false if blocked.
        public bool TryOccupy(GameObject placed)
        {
            if (singleUse && IsOccupied) return false;

            IsOccupied = true;
            Occupant = placed;

            // Disable further clicks if single-use
            if (singleUse && _col) _col.enabled = false;

            // Ensure the occupant knows how to free this node on destroy
            var linker = placed.GetComponent<NodeOccupant>();
            if (!linker) linker = placed.AddComponent<NodeOccupant>();
            linker.Bind(this);

            // settle visuals
            SetTint(idle);
            return true;
        }

        /// Frees the node (called by NodeOccupant when the tower is destroyed/sold).
        public void Release(GameObject from = null)
        {
            if (from && Occupant && from != Occupant) return; // not my guy
            IsOccupied = false;
            Occupant = null;
            if (_col) _col.enabled = true;
            SetTint(idle);
        }

        void SetTint(Color c)
        {
            if (!ringRenderer) return;
            ringRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_BaseColor", c);
            if (_mpb.HasVector("_EmissionColor")) _mpb.SetColor("_EmissionColor", c * 0.5f);
            ringRenderer.SetPropertyBlock(_mpb);
        }
    }
}
