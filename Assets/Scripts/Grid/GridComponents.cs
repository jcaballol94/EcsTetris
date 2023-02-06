using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct GridReference : IComponentData
    {
        public Entity value;
    }

    public struct Position : IComponentData
    {
        public int2 value;
    }

    public struct Rotation : IComponentData
    {
        public int value;

        public int2x2 GetMatrix()
        {
            return RotationMatrix.FromRotation(value);
        }

        public int GetRotated (int delta)
        {
            var newVal = value;
            newVal += delta;
            if (newVal < 0)
                newVal += 4;
            else if (newVal > 3)
                newVal -= 4;

            return newVal;
        }
    }

    public struct RotationMatrix : IComponentData
    {
        public int2x2 value;

        public static int2x2 FromRotation(int rotation)
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

    public struct GridToWorldData : IComponentData
    {
        public float3 origin;
        public float3 up;
        public float3 right;
        public float blockSize;
    }

    public struct GridBounds : IComponentData
    {
        public int2 value;
    }
}