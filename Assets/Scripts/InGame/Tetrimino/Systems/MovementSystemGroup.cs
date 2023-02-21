using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
    [UpdateAfter(typeof(ReadInputSystem))]
    [UpdateBefore(typeof(PlaceTetriminoSystem))]
    public class MovementSystemGroup : ComponentSystemGroup
    {
    }
}
