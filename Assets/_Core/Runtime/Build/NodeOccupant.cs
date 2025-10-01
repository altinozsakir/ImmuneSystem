using UnityEngine;

namespace Core.Build
{
    /// Attach to placed towers automatically; releases the node when this object is destroyed (sell/death).
    public class NodeOccupant : MonoBehaviour
    {
        BuildNode _node;
        public void Bind(BuildNode node) { _node = node; }
        void OnDestroy() { if (_node) _node.Release(gameObject); }
    }
}
