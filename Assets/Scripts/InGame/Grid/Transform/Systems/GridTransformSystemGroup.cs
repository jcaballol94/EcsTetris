using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup), OrderLast = true)]
    public class GridTransformSystemGroup : ComponentSystemGroup
    {
    }
}
