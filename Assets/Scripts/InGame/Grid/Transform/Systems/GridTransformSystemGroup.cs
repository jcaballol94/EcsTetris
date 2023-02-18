using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Unity.Transforms.TransformSystemGroup))]
    [UpdateAfter(typeof(VariableRateSimulationSystemGroup))]
    public class GridTransformSystemGroup : ComponentSystemGroup
    {
    }
}
