using System;
using System.Collections.Generic;
using UnityEngine;
using Core.TimeSystem; // BodyPhase

namespace Core.Waves
{
    [CreateAssetMenu(menuName = "ImmuneTD/Waves/WaveList")]
    public class WaveList : ScriptableObject
    {
        public List<PhaseBlock> blocks = new();
        public PhaseBlock Get(BodyPhase p) => blocks.Find(b => b.phase == p);
    }

    [Serializable]
    public class PhaseBlock
    {
        public BodyPhase phase;
        public List<Wave> waves = new();
    }

    [Serializable]
    public class Wave
    {
        public string name;
        [Tooltip("Seconds after the phase starts")] public float startAfter = 3f;
        [Tooltip("Seconds between spawns (scaled time)")] public float cadence = 0.9f;
        [Tooltip("How many enemies in this wave")] public int count = 6;
        [Tooltip("Lane index (EncounterDirector lanes[] order)")] public int laneIndex = 0;

        [Header("Prefabs")]
        public GameObject enemyPrefab;
        public GameObject elitePrefab;

        [Range(0f,1f)] public float eliteChance = 0.0f;
    }
}
