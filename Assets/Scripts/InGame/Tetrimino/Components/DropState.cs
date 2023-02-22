using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public struct DropState : IComponentData
    {
        public float currentDrop;
        public TetriminoPosition lastCollision;
        public static DropState DefaultDropState => new DropState
        {
            currentDrop = 1f,
            lastCollision = new TetriminoPosition
            {
                position = new int2(int.MaxValue, int.MaxValue),
                orientation = -1
            }
        };

        public bool HasMoved(TetriminoPosition newTrans)
        {
            return newTrans.position.x != lastCollision.position.x
                || newTrans.position.y != lastCollision.position.y
                || newTrans.orientation != lastCollision.orientation;
        }
    }
}