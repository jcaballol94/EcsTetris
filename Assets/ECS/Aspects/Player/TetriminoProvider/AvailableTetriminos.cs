using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct AvailableTetrimnosBlob
    {
        public BlobArray<TetriminoDefinition.Blob> tetriminos;
    }

    public struct AvailableTetriminos : IComponentData
    {
        public BlobAssetReference<AvailableTetrimnosBlob> value;
    }
}