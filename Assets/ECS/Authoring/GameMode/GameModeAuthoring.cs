using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class GameModeAuthoring : MonoBehaviour
    {
        public TetriminoDefinition[] availableTetriminos;
        public Vector2Int spawnPosition;
        public GameObject blockPrefab;
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            AddComponent<ActiveGameModeTag>();

            if (authoring.blockPrefab)
                AddComponent(new BlockPrefab { value = GetEntity(authoring.blockPrefab) });

            // Allocate everything
            var blobBuilder = new BlobBuilder(Unity.Collections.Allocator.Temp);
            ref var blobRoot = ref blobBuilder.ConstructRoot<GameModeDataBlob>();

            blobRoot.spawnPosition = new int2(authoring.spawnPosition.x, authoring.spawnPosition.y);

            {
                var array = blobBuilder.Allocate(ref blobRoot.players, 1);
                array[0] = new PlayerDefinition();
            }

            if (authoring.availableTetriminos != null && authoring.availableTetriminos.Length > 0)
            {
                var array = blobBuilder.Allocate(ref blobRoot.tetriminos, authoring.availableTetriminos.Length);
                
                // Fill the data
                for (int i = 0; i < authoring.availableTetriminos.Length; ++i)
                {
                    DependsOn(authoring.availableTetriminos[i]);
                    authoring.availableTetriminos[i].FillBlob(ref blobBuilder, ref array[i]);
                }
            }

            // Finish up the asset
            var blobAsset = blobBuilder.CreateBlobAssetReference<GameModeDataBlob>(Unity.Collections.Allocator.Persistent);
            blobBuilder.Dispose();

            // Set up the entity
            AddBlobAsset(ref blobAsset, out var hash);
            AddComponent(new GameModeData { value = blobAsset });
        }
    }
}