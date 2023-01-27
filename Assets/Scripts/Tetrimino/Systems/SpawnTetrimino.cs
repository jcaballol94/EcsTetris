using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [UpdateBefore(typeof(PlayerMovement))]
    [UpdateInGroup(typeof(TetrisGameLogic))]
    public partial struct SpawnTetrimino : ISystem
    {
        EntityQuery tetriminoQuery;
        EntityQuery definitionsQuery;

        public void OnCreate(ref SystemState state)
        {
            tetriminoQuery = SystemAPI.QueryBuilder().WithAll<Tetrimino>().Build();
            definitionsQuery = SystemAPI.QueryBuilder().WithAll<TetriminoBlockDefinition, TetriminoOffsetDefinition>().Build();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            // Check if we need to run
            if (tetriminoQuery.CalculateEntityCount() > 0)
                return;

            // Try to get the block data
            if (!SystemAPI.TryGetSingleton(out BlockPrefab blockPrefab))
                return;

            // Get the available pieces
            var definitions = definitionsQuery.ToEntityArray(Allocator.Temp);
            if (definitions.Length == 0)
                return;

            // Get a random one
            var definition = definitions[UnityEngine.Random.Range(0, definitions.Length)];

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            // Create the entity
            var entity = ecb.CreateEntity();
            ecb.AddComponent(entity, new Tetrimino { definition = definition });
            ecb.AddComponent(entity, new PositionInGrid { value = blockPrefab.spawnPos });
            ecb.AddComponent(entity, new Orientation { value = 0 });

            var blockBuffer = ecb.AddBuffer<TetriminoBlockList>(entity);

            // Iterate over all the blocks in the definition
            foreach (var blockPos in SystemAPI.GetBuffer<TetriminoBlockDefinition>(definition))
            {
                var blockEntity = ecb.Instantiate(blockPrefab.value);
                ecb.AddComponent(blockEntity, new LocalBlock { position = blockPos.Value });
                ecb.AddComponent(blockEntity, new ParentTetrimino { value = entity });
                ecb.AppendToBuffer(entity, new TetriminoBlockList { Value = blockEntity });
            }

            definitions.Dispose();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}