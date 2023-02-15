using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct OrientationMatrix : IComponentData
    {
        public int2x2 value;

        public static int2x2 CalculateForRotation(int rotation)
        {
            switch (rotation)
            {
                case 0:
                    return int2x2.identity;
                case 1:
                    return new int2x2(0, -1, 1, 0);
                case 2:
                    return new int2x2(-1, 0, 0, -1);
                case 3:
                    return new int2x2(0, 1, -1, 0);
            }
            return int2x2.identity;
        }
    }
}