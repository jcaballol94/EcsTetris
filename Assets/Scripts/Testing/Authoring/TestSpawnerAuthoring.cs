using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tetris
{
    public class TestSpawnerAuthoring : MonoBehaviour
    {
        public GameObject blockPrefab;
    }

    public class TestSpawnerAuthoringBaking : Baker<TestSpawnerAuthoring>
    {
        public override void Bake(TestSpawnerAuthoring authoring)
        {
            if (authoring.blockPrefab)
            {
                AddComponent(new TestBlockPrefab { value = GetEntity(authoring.blockPrefab) });
            }
        }
    }
}