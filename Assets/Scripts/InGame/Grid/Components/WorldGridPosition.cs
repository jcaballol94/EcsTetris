using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [WriteGroup(typeof(Unity.Transforms.WorldTransform))]
    public struct WorldGridPosition : IComponentData
    {
        public int2 value;
    }
}