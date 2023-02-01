using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class TetriminoDefinition : ScriptableObject
    {
        public Color color = Color.white;
        public Vector2Int[] blocks;
        public Vector2Int[] rotationOffsets;
    }
}
