using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct MoveInput : IComponentData
    {
        public int value;
        public bool changed;
    }
}