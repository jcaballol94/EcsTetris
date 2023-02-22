using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

    #ROOTNAMESPACEBEGIN#
public struct #SCRIPTNAME# : IBufferElementData
{
    public int data;
}

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct #SCRIPTNAME#System : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.AddComponent<#SCRIPTNAME#>(state.SystemHandle);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var buffer = state.EntityManager.GetBuffer<#SCRIPTNAME#>(state.SystemHandle);
        buffer.Clear();
    }
}
#ROOTNAMESPACEEND#