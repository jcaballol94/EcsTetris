using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    public struct TetrisGameMode : IComponentData 
    {
        public Entity blockPrefab;
        public Entity mainGrid;
    }
}
