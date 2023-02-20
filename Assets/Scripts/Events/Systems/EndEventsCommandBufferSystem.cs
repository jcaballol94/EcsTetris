using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(EventsSystemGroup), OrderLast = true)]
    public class EndEventsCommandBufferSystem : EntityCommandBufferSystem
    {
    }
}
