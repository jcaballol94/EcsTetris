using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial struct TestSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSystem = state.World.GetExistingSystemManaged<BeginVariableRateSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer();

            foreach (var (prefab, entity) in SystemAPI
                .Query<TestBlockPrefab>()
                .WithNone<AlreadySpawned>()
                .WithEntityAccess())
            {
                var block = ecb.Instantiate(prefab.value);
                ecb.AddComponent(block, new LocalGridPosition { value = new int2(4, 9) });

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}