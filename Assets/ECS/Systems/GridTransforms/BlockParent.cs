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
    public partial struct BlockParentSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<Transform>(SystemAPI.QueryBuilder()
                .WithAll<LocalPosition>()
                .WithNone<Transform>()
                .Build());

            state.EntityManager.AddComponent<ParentTransform>(SystemAPI.QueryBuilder()
                .WithAll<LocalPosition, ParentTetrimino>()
                .WithNone<ParentTransform>()
                .Build());
        }
    }
}