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

        public float fallSpeed;
        public float fastFallMultiplier;
        public int dropLength;
    }
}