using UnityEngine;

namespace Core.Commander.Build
{
    [CreateAssetMenu(menuName ="ImmuneTD/Commander/BuildableDefinition")]
    public class BuildableDefinition : ScriptableObject
    {
        public GameObject prefab;
        public Vector3 footprintSize = new Vector3(1f,1f,1f);
        public float cost = 0f;
    }
}