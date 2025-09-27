using UnityEngine;


namespace Core.Combat
{
public struct DamagePacket
{
public float amount; // raw damage before resist/marks
public bool execute; // kills regardless of resist when true
public Vector3 hitPoint; // optional for VFX
public int markStacksToAdd; // >= 0 adds marks on hit
public int neutralizeStacksToAdd; // >= 0 adds neutralize on hit
}
}