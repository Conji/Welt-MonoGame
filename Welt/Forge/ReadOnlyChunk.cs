﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Welt.API;
using Welt.API.Forge;
using Welt.Blocks;
using Welt.Core.Forge;
using Welt.Processors;

namespace Welt.Forge
{
    public class ReadOnlyChunk
    {
        internal Chunk Chunk { get; set; }
        
        public Block GetBlock(byte x, byte y, byte z)
        {
            return Chunk.GetBlock(x, y, z);
        }

        public Block GetBlock(int x, int y, int z)
        {
            return Chunk.GetBlock(x, y, z);
        }

        public void SetBlock(byte x, byte y, byte z, Block block)
        {
            Chunk.SetBlock(x, y, z, block);
        }

        public Vector3I GetIndex() => Chunk.Index;
        public Vector3I GetPosition() => Chunk.Position;
    }
}
