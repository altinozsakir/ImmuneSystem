using UnityEngine;

namespace Core.Commander.Build
{
    public class BuildPlacementService
    {
        
        private readonly Transform commander;
        private readonly Camera cam;
        private readonly GridService grid;

        private readonly LayerMask groundMask;
        private readonly LayerMask blockingMask;

        private readonly float blockingRadius;
        private readonly float maxRange;

        public BuildPlacementService(
            Transform commander,
            Camera cam,
            GridService grid,
            LayerMask groundMask,
            LayerMask blockingMask,
            float blockingRadius,
            float maxRange)
        {
            this.commander = commander;
            this.cam = cam;
            this.grid = grid;
            this.groundMask = groundMask;
            this.blockingMask = blockingMask;
            this.blockingRadius = blockingRadius;
            this.maxRange = maxRange;
        }

        public bool TryGetSnappedPoint(Vector2 pointerScreen, out Vector3 snappedWorld)
        {
            snappedWorld = default;

            if(cam==null) return false;

            var ray = cam.ScreenPointToRay(pointerScreen);

            if(!Physics.Raycast(ray, out var hit, 500f, groundMask, QueryTriggerInteraction.Ignore))
                return false;

            var world = hit.point;
            world.y = hit.point.y;
            snappedWorld = grid.SnapXZ(world);
            
            
            return true;
        }

        public bool IsInRange(Vector3 p)
        {
            Vector3 a = commander.position;
            a.y = 0f;
            Vector3 b = p;
            b.y = 0f;

            return (b-a).sqrMagnitude <= (maxRange * maxRange);
        }

                public bool IsBlocked(Vector3 p)
        {
            // Any blocking collider in radius?
            var hits = Physics.OverlapSphere(p, blockingRadius, blockingMask, QueryTriggerInteraction.Ignore);
            return hits != null && hits.Length > 0;
        }

        public bool IsValidPlacement(Vector3 p)
        {
            if (!IsInRange(p)) return false;
            if (IsBlocked(p)) return false;
            return true;
        }

        public GameObject Build(BuildableDefinition def, Vector3 p, Quaternion r)
        {
            if (def == null || def.prefab == null) return null;
            return Object.Instantiate(def.prefab, p, r);
        }


    }
}