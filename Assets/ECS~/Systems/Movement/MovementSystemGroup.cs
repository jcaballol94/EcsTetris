using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(GridCollisionsSystemGroup))]
    public class MovementSystemGroup : ComponentSystemGroup
    {
    }
}
