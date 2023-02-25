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
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(GenerateRandomTetriminosSystem))]
    [UpdateBefore(typeof(SpawnPreviewBlocksSystem))]
    [UpdateBefore(typeof(SpawnTetriminoSystem))]
    public partial struct SpawnNextPreviewSystem : ISystem
    {
        private EntityArchetype m_previewArchetype;

        public void OnCreate(ref SystemState state)
        {
            m_previewArchetype = state.EntityManager.CreateArchetype(typeof(PreviewTag), typeof(TetriminoData), typeof(GridRef), typeof(SceneTag));
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (grid, scene, entity) in SystemAPI
                .Query<RefRO<NextGrid>, SceneTag>()
                .WithNone<NextPreview>()
                .WithEntityAccess())
            {
                var preview = ecb.CreateEntity(m_previewArchetype);
                ecb.SetName(preview, "NextPreview");
                ecb.SetComponent(preview, new GridRef { value = grid.ValueRO.entity });
                ecb.SetSharedComponent(preview, scene);

                ecb.AddComponent(entity, new NextPreview { value = preview });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}