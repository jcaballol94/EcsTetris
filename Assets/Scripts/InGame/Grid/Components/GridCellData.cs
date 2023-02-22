using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [InternalBufferCapacity(0)]
    public struct GridCellData : IBufferElementData
    {
        public bool available;

        public static GridCellData Empty = new GridCellData { available = true };
        public static GridCellData Busy = new GridCellData { available = false };
    }
}