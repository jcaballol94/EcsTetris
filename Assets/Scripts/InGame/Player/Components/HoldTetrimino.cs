using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct HoldTetrimino : IComponentData
    {
        public TetriminoData data;
    }
}