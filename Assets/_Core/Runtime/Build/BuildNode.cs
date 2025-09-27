// Assets/_Core/Runtime/Build/BuildNode.cs
using UnityEngine;
#if UNITY_600_0_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace Core.Build
{
    public enum SlotType { Capillary, Lymph, Barrier }

    [RequireComponent(typeof(Collider))]
    public class BuildNode : MonoBehaviour
    {
        public SlotType slotType;
        public bool occupied;

        [Header("Indicator Mode")]
        public bool useSpriteIndicator = false; // off = 3D mesh, on = sprite

        [Header("3D Mesh Indicator (when useSpriteIndicator = false)")]
        public Renderer meshIndicator; // MeshRenderer for a ring/puck mesh
        public bool meshUseEmission = true;
        public float meshEmissionBoost = 1.5f;

        [Header("Sprite Indicator (when useSpriteIndicator = true)")]
        public SpriteRenderer spriteIndicator; // 2D sprite used as a decal/marker
#if UNITY_600_0_OR_NEWER
        public SortingGroup spriteSorting; // optional, helps control draw order
#endif

        [Header("Colors")] public Color baseColor = new(0.85f, 0.85f, 0.85f, 1f);
        public Color hoverColor = new(0.3f, 1f, 0.6f, 1f);
        public Color blockedColor = new(1f, 0.35f, 0.35f, 1f);

        static readonly int _BaseColor = Shader.PropertyToID("_BaseColor");
        static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        MaterialPropertyBlock _mpb;

        void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            ApplyColor(baseColor);
        }

        void OnValidate()
        {
            if (Application.isPlaying) return;
            if (_mpb == null) _mpb = new MaterialPropertyBlock();
            ApplyColor(baseColor);
        }

        public void SetHover(bool canPlace, bool isHover)
        {
            var c = isHover ? (canPlace ? hoverColor : blockedColor) : baseColor;
            ApplyColor(c);
        }

        void ApplyColor(Color c)
        {
            if (useSpriteIndicator)
            {
                if (!spriteIndicator) return;
                // Primary color path for sprites
                spriteIndicator.color = c;
                // Also try to push color/emission to material if supported
                var mat = spriteIndicator.sharedMaterial;
                if (mat && mat.HasProperty(_BaseColor))
                {
                    spriteIndicator.GetPropertyBlock(_mpb);
                    _mpb.SetColor(_BaseColor, c);
                    if (mat.HasProperty(_EmissionColor))
                        _mpb.SetColor(_EmissionColor, c * 1.25f);
                    spriteIndicator.SetPropertyBlock(_mpb);
                }
                return;
            }

            // Mesh path
            if (!meshIndicator) return;
            meshIndicator.GetPropertyBlock(_mpb);
            _mpb.SetColor(_BaseColor, c);
            if (meshUseEmission)
            {
                var e = c * meshEmissionBoost; e.a = 1f;
                _mpb.SetColor(_EmissionColor, e);
            }
            meshIndicator.SetPropertyBlock(_mpb);
        }

        void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("BuildSlot");
        }
    }
}