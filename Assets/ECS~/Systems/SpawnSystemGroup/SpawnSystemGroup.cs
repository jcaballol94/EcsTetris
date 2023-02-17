using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginVariableRateSimulationEntityCommandBufferSystem))]
    public class SpawnSystemGroup : ComponentSystemGroup
    {
    }
}
