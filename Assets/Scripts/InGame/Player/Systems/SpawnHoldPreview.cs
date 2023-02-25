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
    [UpdateBefore(typeof(SpawnPreviewBlocksSystem))]
    public partial struct SpawnHoldPreviewSystem : ISystem
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
                .Query<RefRO<HoldGrid>, SceneTag>()
                .WithAll<HoldTetrimino>()
                .WithNone<HoldPreview>()
                .WithEntityAccess())
            {
                var preview = ecb.CreateEntity(m_previewArchetype);
                ecb.SetName(preview, "HoldPreview");
                ecb.SetComponent(preview, new GridRef { value = grid.ValueRO.value });
                ecb.SetSharedComponent(preview, scene);

                ecb.AddComponent(entity, new HoldPreview { value = preview });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}