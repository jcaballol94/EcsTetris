using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class ManagedInput : IComponentData, IDisposable
    {
        public TetrisInput value;

        public void Dispose()
        {
            if (value != null)
            {
                value.Dispose();
                value = null;
            }
        }
    }
}