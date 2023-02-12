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
        private EntityQuery m_addPositionQuery;
        private EntityQuery m_addParentPositionQuery;

        public void OnCreate(ref SystemState state)
        {
            m_addPositionQuery = SystemAPI.QueryBuilder().WithAll<LocalPosition>().WithNone<Position>().Build();
            m_addParentPositionQuery = SystemAPI.QueryBuilder().WithAll<LocalPosition, ParentTetrimino>().WithNone<ParentPosition>().Build();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<Position>(m_addPositionQuery);
            state.EntityManager.AddComponent<ParentPosition>(m_addParentPositionQuery);
        }
    }
}