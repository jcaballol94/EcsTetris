using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct GameData : IComponentData
    {
        public int2 spawnPosition;
        public float baseSpawnDelay;
        public float spawnDelayDelta;

        public float moveRepeatDelay;
        public float moveRepeatPeriod;

        public BlobAssetReference<GameModeAuthoring.SpeedsBlob> fallSpeed;
        public float fastFallMultiplier;
        public int dropLength;

        public float GetSpeedForLevel (int level)
        {
            var idx = math.clamp(level - 1, 0, fallSpeed.Value.values.Length - 1);
            return fallSpeed.Value.values[idx];
        }
    }
}