using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct UnmanagedInput : IComponentData
    {
        public int moveValue;
        public bool movePressed;
        public float timeSinceLastMove;
        public bool repeatingMove;

        public int rotateValue;
        public bool rotatePressed;

        public bool fallFast;
        public bool drop;
        public bool prevDrop;
    }
}