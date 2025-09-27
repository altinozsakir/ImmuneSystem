// Assets/_Core/Runtime/Pathing/BranchPoint.cs
using UnityEngine;
using UnityEngine.Splines;


namespace Core.Pathing
{
public class BranchPoint : MonoBehaviour
{
public SplineContainer lane; // this lane
[Range(0,1)] public float t; // location along lane
public SplineContainer[] nextLanes; // lanes we could switch to later
}
}