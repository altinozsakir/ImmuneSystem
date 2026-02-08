using UnityEngine;

namespace Core.Commander.Build
{
    public class BuildGhostController
    {
        private readonly GameObject ghost;
        private readonly Renderer[] renderers;
        private readonly Material validMat;
        private readonly Material invalidMat;

        public bool Exists => ghost != null;

        public BuildGhostController(GameObject prefab, Material validMat, Material invalidMat)
        {
            ghost = Object.Instantiate(prefab);
            ghost.name = prefab.name + "_GHOST";
            ghost.layer = LayerMask.NameToLayer("Ignore Raycast"); // safe default
            renderers = ghost.GetComponentsInChildren<Renderer>(true);
            this.validMat = validMat;
            this.invalidMat = invalidMat;

            // Ensure it doesn't collide
            foreach (var c in ghost.GetComponentsInChildren<Collider>(true))
                c.enabled = false;
        }

        public void SetPose(Vector3 position, Quaternion rotation)
        {
            if (!ghost) return;
            ghost.transform.SetPositionAndRotation(position, rotation);
        }

        public void SetValid(bool isValid)
        {
            if (renderers == null) return;
            var mat = isValid ? validMat : invalidMat;
            if (mat == null) return;

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sharedMaterial = mat;
        }

        public void Destroy()
        {
            if (ghost) Object.Destroy(ghost);
        }
    }
}