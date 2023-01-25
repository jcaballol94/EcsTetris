using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
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

            // Get the available pieces
            var definitions = definitionsQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            if (definitions.Length == 0)
                return;

            // Get a random one
            var idx = UnityEngine.Random.Range(0, definitions.Length);

            // Create the entity
            var entity = state.EntityManager.CreateEntity(typeof(Tetrimino));
            SystemAPI.SetComponent(entity, new Tetrimino { definition = definitions[idx] });

            definitions.Dispose();
        }
    }
}