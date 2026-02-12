using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Waves
{
    [CreateAssetMenu(menuName = "ImmuneTD/Waves/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        public List<Wave> waves = new();

        [Serializable]
        public class Wave
        {
            public string name = "Wave";
            [Min(0f)] public float preDelay = 2f;   // delay before wave starts
            public List<SpawnGroup> groups = new(); // multiple enemy types per wave
            [Min(0f)] public float postDelay = 3f;  // delay after finishing spawns
        }

        [Serializable]
        public class SpawnGroup
        {
            public GameObject enemyPrefab;
            [Min(1)] public int count = 10;
            [Min(0.01f)] public float interval = 0.5f;
            [Tooltip("Optional lane name; must match SpawnPoint.lane")]
            public string lane = "Default";
        }
    }
}
