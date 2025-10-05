using UnityEngine;


namespace Core.Combat
{
    // Teams that an entity can belong to
    public enum Team { Neutral, Player, Enemy }

    // Types of threat sources in the game
    public enum ThreatClass { Goal, Tower, Wall, Decoy, Unit }

    // Interface for objects that can take damage
    public interface IHittable
    {
        // Apply damage to the object
        void TakeDamage(float amount);

        // Reference to the object's transform
        Transform transform { get; }

        bool IsAlive { get; }

    }

    // Interface for objects that are sources of threat
    public interface IThreatSource : IHittable
    {
        // The threat class of the object
        ThreatClass Class { get; }

        // The base priority for targeting this object (higher means more attractive target)
        int StaticPriority { get; }        // base pull (Goal < Tower < Decoy)
    }
}