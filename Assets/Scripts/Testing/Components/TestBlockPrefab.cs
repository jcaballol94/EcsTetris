using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct TestBlockPrefab : IComponentData
    {
        public Entity value;
        public Entity grid;
    }
}