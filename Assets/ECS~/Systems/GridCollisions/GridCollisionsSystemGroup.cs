using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateAfter(typeof(SpawnSystemGroup))]
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    public class GridCollisionsSystemGroup : ComponentSystemGroup
    {
    }
}
