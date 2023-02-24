using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class PausePanelReference : IComponentData
    {
        public GameObjectVariable value;
    }
}