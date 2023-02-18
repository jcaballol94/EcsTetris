using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct TetriminoData : IComponentData
    {
        public BlobAssetReference<TetriminoDefinition.Blob> asset;

        public float4 color => asset.Value.color;
        public ref BlobArray<OrientationOffsets.Blob> offsets => ref asset.Value.rotationOffsets;
        public ref BlobArray<int2> blocks => ref asset.Value.blocks;
    }
}