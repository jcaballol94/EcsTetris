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
    }

    public class GameModeAuthoringBaking : Baker<GameModeAuthoring>
    {
        public override void Bake(GameModeAuthoring authoring)
        {
            AddComponent<ActiveGameModeTag>();

            // Add the players
            var playersBuffer = AddBuffer<PlayerDefinitionBuffer>();
            playersBuffer.Add(new PlayerDefinitionBuffer());

            if (authoring.availableTetriminos != null && authoring.availableTetriminos.Length > 0)
            {
                // Allocate everything
                var blobBuilder = new BlobBuilder(Unity.Collections.Allocator.Temp);
                ref var blobRoot = ref blobBuilder.ConstructRoot<AvailableTetrimnosBlob>();
                var array = blobBuilder.Allocate(ref blobRoot.tetriminos, authoring.availableTetriminos.Length);
                
                // Fill the data
                for (int i = 0; i < authoring.availableTetriminos.Length; ++i)
                {
                    DependsOn(authoring.availableTetriminos[i]);
                    authoring.availableTetriminos[i].FillBlob(ref blobBuilder, ref array[i]);
                }

                // Finish up the asset
                var blobAsset = blobBuilder.CreateBlobAssetReference<AvailableTetrimnosBlob>(Unity.Collections.Allocator.Persistent);
                blobBuilder.Dispose();

                // Set up the entity
                AddBlobAsset(ref blobAsset, out var hash);
                AddComponent(new AvailableTetriminos { value = blobAsset });
            }
        }
    }
}