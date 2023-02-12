using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [RequireMatchingQueriesForUpdate]
    [UpdateInGroup(typeof(GridTransformsSystemGroup))]
    public partial struct AddParentPositionComponentSystem : ISystem
    {
        private EntityQuery m_query;

        public void OnCreate(ref SystemState state)
        {
            m_query = SystemAPI.QueryBuilder().WithAll<ParentTetrimino, LocalPosition>().WithNone<ParentPosition>().Build();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<ParentPosition>(m_query);
        }
    }
}