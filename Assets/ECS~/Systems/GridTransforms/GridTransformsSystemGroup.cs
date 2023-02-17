using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndVariableRateSimulationEntityCommandBufferSystem))]
    public class GridTransformsSystemGroup : ComponentSystemGroup
    {
    }
}
