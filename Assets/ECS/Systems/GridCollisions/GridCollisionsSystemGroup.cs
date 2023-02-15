using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateAfter(typeof(AfterInitialCommandBufferSystemGroup))]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public class GridCollisionsSystemGroup : ComponentSystemGroup
    {
    }
}
