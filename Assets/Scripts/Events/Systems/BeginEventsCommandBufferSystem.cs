using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(EventsSystemGroup), OrderFirst = true)]
    public class BeginEventsCommandBufferSystem : EntityCommandBufferSystem
    {
    }
}
