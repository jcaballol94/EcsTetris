using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [System.Serializable]
    public struct OrientationOffsets
    {
        public Vector2Int[] offsets;

        // The binary representation
        public struct Blob
        {
            public BlobArray<int2> offsets;
        }

        // Fill the binary representation at baking time
        public void FillBlob(ref BlobBuilder builder, ref Blob dst)
        {
            var arrayBuilder = builder.Allocate(ref dst.offsets, offsets.Length);
            for (int i = 0; i < offsets.Length; ++i)
            {
                arrayBuilder[i] = new int2(offsets[i].x, offsets[i].y);
            }
        }
    }

    public class TetriminoDefinition : ScriptableObject
    {
        public struct Blob
        {
            public float4 color;
            public BlobArray<int2> blocks;
            public BlobArray<OrientationOffsets.Blob> rotationOffsets;
        }

        public Color color = Color.white;
        public Vector2Int[] blocks;
        public OrientationOffsets[] rotationOffsets;

        public void FillBlob(ref BlobBuilder builder, ref Blob dst)
        {
            dst.color = new float4(color.r, color.g, color.b, color.a);

            var blocksArray = builder.Allocate(ref dst.blocks, blocks.Length);
            for (int i = 0; i < blocks.Length; ++i)
            {
                blocksArray[i] = new int2(blocks[i].x, blocks[i].y);
            }

            var rotationsArray = builder.Allocate(ref dst.rotationOffsets, rotationOffsets.Length);
            for (int i = 0; i < rotationOffsets.Length; ++i)
            {
                rotationOffsets[i].FillBlob(ref builder, ref rotationsArray[i]);
            }
        }
    }
}
