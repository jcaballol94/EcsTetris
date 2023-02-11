using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(EndVariableRateSimulationEntityCommandBufferSystem))]
    public class AfterFinalCommandBufferSystemGroup : ComponentSystemGroup
    {
    }
}
