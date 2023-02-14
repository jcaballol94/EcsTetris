using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(InputSystemGroup), OrderFirst = true)]
    public partial struct AddManagedInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder().WithAll<InputReader>().WithNone<ManagedInput>().Build();
            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var entity in entities)
            {
                var input = new TetrisInput();
                input.Enable();
                input.Game.Enable();
                state.EntityManager.AddComponentObject(entity, new ManagedInput { value = input });
            }

            entities.Dispose();
        }
    }
}