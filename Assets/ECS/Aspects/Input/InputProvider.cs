using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct InputProvider : IComponentData
    {
        public Entity value;
    }

    public struct PrevInputProvider : ICleanupComponentData
    {
        public Entity value;
    }
}