using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class GameInputComponent : IComponentData
    {
        public TetrisInput value;
    }
}