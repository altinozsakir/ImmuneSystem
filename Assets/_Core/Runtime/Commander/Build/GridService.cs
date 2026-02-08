using UnityEngine;

namespace Core.Commander.Build
{
    public class GridService
    {
        private readonly float cellSize;
        private readonly Vector3 origin;

        public GridService(float cellSize, Vector3 origin)
        {
            this.cellSize = Mathf.Max(0.01f,cellSize);
            this.origin = origin;
        }

        public Vector3 SnapXZ(Vector3 world)
        {
            float x = Mathf.Round((world.x - origin.x) / cellSize) * cellSize + origin.x;
            float z = Mathf.Round((world.z - origin.z) / cellSize) * cellSize + origin.z;

            return new Vector3(x,world.y, z);

        }
    }
}