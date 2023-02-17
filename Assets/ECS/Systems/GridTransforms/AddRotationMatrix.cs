using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(AfterInitialCommandBufferSystemGroup))]
    [UpdateAfter(typeof(SpawnTetriminoSystem))]
    public partial struct AddRotationMatrixSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (orientation, entity)
                in SystemAPI.Query<Orientation>()
                .WithNone<OrientationMatrix>()
                .WithEntityAccess())
            {
                ecb.AddComponent(entity, new OrientationMatrix { value = OrientationMatrix.CalculateForRotation(orientation.value) });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}