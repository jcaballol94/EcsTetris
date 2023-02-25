using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct PlayerData : IComponentData
    {
        public Entity mainGrid;
        public Entity nextGrid;
        public Entity holdGrid;
    }
}