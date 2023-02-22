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
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct UpdateGridCollisionsSystem : ISystem
    {
        [BurstCompile]
        [WithAll(typeof(StaticBlockTag))]
        public partial struct UpdateGridCollisionsJob : IJobEntity
        {
            public DynamicBuffer<GridCellData> gridCells;
            public int2 bounds;

            [BurstCompile]
            private void Execute(in BlockPosition transform)
            {
                var idx = transform.position.y * bounds.x + transform.position.x;
                gridCells[idx] = GridCellData.Busy;
            }
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RefreshGridCollisionsEvent>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<RefreshGridCollisionsEvent> events, true)) return;
            if (events.Length == 0) return;

            var query = SystemAPI.QueryBuilder()
                .WithAll<BlockPosition, StaticBlockTag, GridRef>()
                .Build();

            foreach (var ev in events)
            {
                query.ResetFilter();
                query.AddSharedComponentFilter(new GridRef { value = ev.grid });

                var cells = state.EntityManager.GetBuffer<GridCellData>(ev.grid);
                var bounds = state.EntityManager.GetComponentData<GridBounds>(ev.grid);

                // Reset the grid
                for (int i = 0; i < cells.Length; ++i)
                {
                    cells[i] = GridCellData.Empty;
                }

                // Fill with the new values
                new UpdateGridCollisionsJob
                {
                    bounds = bounds.size,
                    gridCells = cells
                }.Run();
            }
        }
    }
}