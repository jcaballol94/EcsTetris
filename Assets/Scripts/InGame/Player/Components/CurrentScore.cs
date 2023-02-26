using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct CurrentScore : IComponentData
    {
        public int score;
        public int level;
        public int lines;

        public static readonly CurrentScore Default = new CurrentScore { score = 0, level = 1, lines = 0 };
    }
}