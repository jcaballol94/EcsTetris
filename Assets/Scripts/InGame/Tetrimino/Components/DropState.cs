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
        public LocalGridTransform lastCollision;
        public static DropState DefaultDropState => new DropState
        {
            currentDrop = 1f,
            lastCollision = new LocalGridTransform
            {
                position = new int2(int.MaxValue, int.MaxValue),
                orientation = -1
            }
        };

        public bool HasMoved(LocalGridTransform newTrans)
        {
            return newTrans.position.x != lastCollision.position.x
                || newTrans.position.y != lastCollision.position.y
                || newTrans.orientation != lastCollision.orientation;
        }
    }
}