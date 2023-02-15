using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GridTransformsSystemGroup), OrderFirst = true)]
    public partial struct AddGridToWorldDataSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var dataLookup = SystemAPI.GetComponentLookup<GridToWorldData>(true);

            // Entities that have a grid position and a unity transform, add the data required for the translation
            foreach (var (grid, entity) in SystemAPI
                .Query<GridRef>()
                .WithAll<Transform, Unity.Transforms.LocalTransform>()
                .WithNone<GridToWorldData>()
                .WithEntityAccess())
            {
                ecb.AddComponent(entity, dataLookup[grid.value]);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}