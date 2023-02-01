using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct TetrisGameMode : IComponentData 
    {
        public Entity blockPrefab;
        public Entity mainGrid;
        public int2 spawnPosition;
    }
}
