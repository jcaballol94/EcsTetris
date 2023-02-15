using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(MovementSystemGroup))]
    public class BlockPlacingSystemGroup : ComponentSystemGroup
    {
    }
}
