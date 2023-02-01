using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    [AddComponentMenu("Tetris/Grid/Grid")]
    public class GridAuthoring : MonoBehaviour
    {
        public float BlockSize = 1f;
    }
}