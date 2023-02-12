using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct GameModeDataBlob
    {
        public BlobArray<TetriminoDefinition.Blob> tetriminos;
        public BlobArray<PlayerDefinition> players;
        public int2 spawnPosition;
    }

    public struct GameModeData : IComponentData
    {
        public BlobAssetReference<GameModeDataBlob> value;

        public ref TetriminoDefinition.Blob GetTetriminoDefinition(int idx)
        {
            return ref value.Value.tetriminos[idx];
        }

        public ref TetriminoDefinition.Blob GetTetriminoDefinition(in TetriminoType type)
        {
            return ref GetTetriminoDefinition(type.idx);
        }
    }
}