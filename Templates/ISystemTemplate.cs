using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

    #ROOTNAMESPACEBEGIN#
[BurstCompile]
[RequireMatchingQueriesForUpdate]
public partial struct #SCRIPTNAME# : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct #SCRIPTNAME#Job : IJobEntity
{
    [BurstCompile]
    private void Execute()
    {
    }
}
#ROOTNAMESPACEEND#