﻿#region Copyright
// COPYRIGHT 2015 JUSTIN COX (CONJI)
#endregion
namespace Welt.Forge.Generators
{
    internal class TerrainWithCaves : SimpleTerrain
    {
        #region generateTerrain

        protected override sealed void GenerateTerrain(Chunk chunk, byte x, byte z, uint blockX, uint blockZ)
        {
            var groundHeight = (int) GetBlockNoise(blockX, blockZ);
            if (groundHeight < 1)
            {
                groundHeight = 1;
            }
            else if (groundHeight > 128)
            {
                groundHeight = 96;
            }

            // Default to sunlit.. for caves
            var sunlit = true;

            var blockType = BlockType.None;

            //chunk.Blocks[x, groundHeight, z] = new Block(BlockType.Grass,true);
            //chunk.Blocks[x, 0, z] = new Block(BlockType.Dirt, true);

            var offset = x*Chunk.FlattenOffset + z*Chunk.Size.Y;
            chunk.Blocks[offset + groundHeight] = new Block(BlockType.Grass);
            chunk.Blocks[offset] = new Block(BlockType.Dirt);

            for (int y = Chunk.Max.Y; y >= 0; y--)
            {
                if (y > groundHeight)
                {
                    blockType = BlockType.None;
                }
                // Or we at or below ground height?
                else if (y < groundHeight)
                {
                    // Since we are at or below ground height, let's see if we need
                    // to make
                    // a cave
                    var noiseX = (blockX + (uint) World.Seed);
                    var octave1 = PerlinSimplexNoise.Noise(noiseX*0.009f, blockZ*0.009f, y*0.009f)*0.25f;

                    var initialNoise = octave1 + PerlinSimplexNoise.Noise(noiseX*0.04f, blockZ*0.04f, y*0.04f)*0.15f;
                    initialNoise += PerlinSimplexNoise.Noise(noiseX*0.08f, blockZ*0.08f, y*0.08f)*0.05f;

                    if (initialNoise > 0.2f)
                    {
                        blockType = BlockType.None;
                    }
                    else
                    {
                        // We've placed a block of dirt instead...nothing below us
                        // will be sunlit
                        if (sunlit)
                        {
                            sunlit = false;
                            blockType = BlockType.Grass;
                            //chunk.addGrassBlock(x,y,z);
                        }
                        else
                        {
                            blockType = BlockType.Dirt;
                            if (octave1 < 0.2f)
                            {
                                blockType = BlockType.Rock;
                            }
                        }
                    }
                }

                chunk.SetBlock(x, (byte) y, z, new Block(blockType));
            }
        }

        #endregion

        private float GetBlockNoise(uint blockX, uint blockZ)
        {
            var mediumDetail = PerlinSimplexNoise.Noise(blockX/300.0f, blockZ/300.0f, 20);
            var fineDetail = PerlinSimplexNoise.Noise(blockX/80.0f, blockZ/80.0f, 30);
            var bigDetails = PerlinSimplexNoise.Noise(blockX/800.0f, blockZ/800.0f);
            var noise = bigDetails*64.0f + mediumDetail*32.0f + fineDetail*16.0f; // *(bigDetails
            // *
            // 64.0f);

            return noise + 16;
        }
    }
}