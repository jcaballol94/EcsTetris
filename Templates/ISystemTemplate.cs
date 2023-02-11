using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

    #ROOTNAMESPACEBEGIN#
[BurstCompile]
[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
public partial struct #SCRIPTNAME#System : ISystem
{
    [BurstCompile]
    public partial struct #SCRIPTNAME#Job : IJobEntity
    {
        [BurstCompile]
        private void Execute()
        {
        }
    }

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
#ROOTNAMESPACEEND#