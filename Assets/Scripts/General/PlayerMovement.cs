using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Tetris
{
    [UpdateInGroup(typeof(TetrisGameLogic))]
    [UpdateAfter(typeof(SpawnTetrimino))]
    public class PlayerMovement : ComponentSystemGroup
    {
    }
}
