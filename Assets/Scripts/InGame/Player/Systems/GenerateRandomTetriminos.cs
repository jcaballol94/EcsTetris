using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnPlayerSystem))]
    public partial struct GenerateRandomTetriminosSystem : ISystem
    {
        [BurstCompile]
        [WithNone(typeof(NextTetrimino))]
        public partial struct GenerateRandomTetriminosJob : IJobEntity
        {
            [ReadOnly] public DynamicBuffer<AvailableTetriminos> availableTetriminos;

            public EntityCommandBuffer ecb;

            [BurstCompile]
            private void Execute(Entity entity, ref DynamicBuffer<TetriminoQueue> queue, ref RandomProvider random)
            {
                // We only need to generate them if it is empty
                if (queue.Length == 0)
                {
                    // Fill the array
                    foreach (var tetrimino in availableTetriminos)
                    {
                        queue.Add(new TetriminoQueue { data = new TetriminoData { asset = tetrimino.asset } });
                    }

                    // Shuffle them around
                    for (int i = 0; i < queue.Length; ++i)
                    {
                        Swap(ref queue, i, i + math.abs(random.value.NextInt()) % (queue.Length - i));
                    }
                }

                // Select the next
                ecb.AddComponent(entity, new NextTetrimino { data = queue[0].data });
                queue.RemoveAtSwapBack(0);
            }

            private void Swap(ref DynamicBuffer<TetriminoQueue> queue, int a, int b)
            {
                var aux = queue[a];
                queue[a] = queue[b];
                queue[b] = aux;
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AvailableTetriminos>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<AvailableTetriminos> tetriminos, true)) return;

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            new GenerateRandomTetriminosJob
            {
                ecb = ecb,
                availableTetriminos = tetriminos
            }.Run();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}