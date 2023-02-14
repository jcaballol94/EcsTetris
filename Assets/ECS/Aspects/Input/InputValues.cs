using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct InputValues : IComponentData
    {
        public int moveValue;
        public bool movePressed;
    }
}