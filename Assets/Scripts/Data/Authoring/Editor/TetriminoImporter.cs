using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Tetris
{
    [ScriptedImporter(1, "tetrimino")]
    public class TetriminoImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var fileStream = System.IO.File.OpenText(assetPath);
            try
            {
                var tetrimino = ScriptableObject.CreateInstance<TetriminoDefinition>();
                ctx.AddObjectToAsset("tetrimino", tetrimino);
                ctx.SetMainObject(tetrimino);

                // Import the block positions
                tetrimino.blocks = new Vector2Int[4];
                {
                    var blockIdx = 0;
                    for (int i = 0; i < 5 && !fileStream.EndOfStream; ++i)
                    {
                        var line = fileStream.ReadLine();
                        for (int j = 0; j < line.Length; ++j)
                        {
                            if (line[j] == 'X')
                                tetrimino.blocks[blockIdx++] = new Vector2Int(j - 2, 2 - i);
                        }
                    }
                    Debug.Assert(blockIdx == 4, "A tetrimino should have 4 blocks!");
                }

                tetrimino.rotationOffsets = new Vector2Int[5 * 4];
                for (int i = 0; i < 4; ++i)
                {
                    var line = fileStream.ReadLine();
                    var offsets = line.Split('\t');
                    for (int j = 0; j < 5; ++j)
                    {
                        var nums = offsets[j].Split(',');
                        tetrimino.rotationOffsets[i * 4 + j] = new Vector2Int(int.Parse(nums[0]), int.Parse(nums[1]));
                    }
                }
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
