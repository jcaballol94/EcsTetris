using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct TetriminoPosition : IComponentData
    {
        public int2 position;
        public int orientation;
    }
}