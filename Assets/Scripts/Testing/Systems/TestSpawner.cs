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
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
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
                ecb.Instantiate(prefab.value);

                ecb.AddComponent<AlreadySpawned>(entity);
            }
        }
    }
}