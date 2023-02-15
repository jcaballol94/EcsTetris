using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct GameSettings : ISharedComponentData
    {
        public int2 spawnPosition;
        public float moveRepeatDelay;
        public float moveRepeatRatio;
    }
}