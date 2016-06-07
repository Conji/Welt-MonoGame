﻿#region Copyright
// COPYRIGHT 2015 JUSTIN COX (CONJI)
#endregion
#region Using Statements

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Welt.Blocks;
using Welt.Models;
using Welt.Types;

#endregion

namespace Welt.Forge
{
    public class Chunk
    {
        private const byte MAX_SUN_VALUE = 16;

        public Chunk(World world, Vector3I index)
        {
            this.World = world;
            Blocks = new Block[Size.X*Size.Z*Size.Y];
            HeightMap = new byte[16, 16];
            VertexList = new List<VertexPositionTextureLight>();
            WaterVertexList = new List<VertexPositionTextureLight>();
            GrassVertexList = new List<VertexPositionTextureLight>();
            IndexList = new List<short>();
            WaterIndexList = new List<short>();
            GrassIndexList = new List<short>();

            Assign(index);
            /* world.viewableChunks[index.X, index.Z] = this;
             dirty = true;
             this.Index = index;
             this.Position = new Vector3i(index.X * SIZE.X, index.Y * SIZE.Y, index.Z * SIZE.Z);
             this._boundingBox = new BoundingBox(new Vector3(Position.X, Position.Y, Position.Z), new Vector3(Position.X + SIZE.X, Position.Y + SIZE.Y, Position.Z + SIZE.Z));
             */
        }

        public ChunkState State { get; set; }
        public BoundingBox BoundingBox { get; private set; }

        public void Assign(Vector3I index)
        {
            //ensure world is set directly in here to have access to N S E W as soon as possible

            World.Chunks.Remove(Index.X, Index.Z);
            World.Chunks[index.X, index.Z] = this;

            Dirty = true;
            //Array.Clear(Blocks, 0, Blocks.Length);
            Index = index;
            Position = new Vector3I(index.X*Size.X, index.Y*Size.Y, index.Z*Size.Z);
            BoundingBox = new BoundingBox(new Vector3(Position.X, Position.Y, Position.Z),
                new Vector3(Position.X + Size.X, Position.Y + Size.Y, Position.Z + Size.Z));

            //TODO next optimization step would be reusing the vertexbuffer
            //vertexList.Clear(); 
            //indexList.Clear();
        }

        public void Clear()
        {
            VertexList.Clear();
            IndexList.Clear();

            WaterVertexList.Clear();
            WaterIndexList.Clear();

            VertexCount = 0;
            WaterVertexCount = 0;
        }

        #region SetBlock

        public void SetBlock(byte x, byte y, byte z, Block b)
        {
            if (b.Id == BlockType.Water)
            {
                if (LowestNoneBlock.Y > y)
                {
                    LowestNoneBlock = new Vector3B(x, y, z);
                }
            }

            if (b.Id == BlockType.None)
            {
                if (LowestNoneBlock.Y > y)
                {
                    LowestNoneBlock = new Vector3B(x, y, z);
                }
            }
            else if (HighestSolidBlock.Y < y)
            {
                HighestSolidBlock = new Vector3B(x, y, z);
                HeightMap[x, z] = y;
            }

            //comment this line : you should have nothing on screen, else you ve been setting blocks directly in array !
            try
            {
                Blocks[x*FlattenOffset + z*Size.Y + y] = b;
            }
            catch (IndexOutOfRangeException e)
            {
                
            }
            Dirty = true;
        }

        #endregion

        public bool OutOfBounds(byte x, byte y, byte z)
        {
            return x >= Size.X || y >= Size.Y || z >= Size.Z;
        }

        #region GetBlock

        public Block GetBlock(int relx, int rely, int relz)
        {
            if (rely < 0 || rely > Max.Y)
            {
                //infinite Y : y bounds currently set as rock for never rendering those y bounds
                return new Block(BlockType.Rock);
            }

            //handle the normal simple case
            if (relx >= 0 && relz >= 0 && relx < Size.X && relz < Size.Z)
            {
                var b = Blocks[relx*FlattenOffset + relz*Size.Y + rely];
                return b;
            }

            //handle all special cases

            int x = relx, z = relz;
            Chunk nChunk = null;

            //TODO chunk relative GetBlock could even handle more tha just -1 but -2 -3 ... -15 

            if (relx < 0) x = Max.X;
            if (relz < 0) z = Max.Z;
            if (relx > 15) x = 0;
            if (relz > 15) z = 0;


            if (x != relx && x == 0)
                if (z != relz && z == 0)
                    nChunk = Nw;
                else if (z != relz && z == 15)
                    nChunk = Sw;
                else
                    nChunk = W;
            else if (x != relx && x == 15)
                if (z != relz && z == 0)
                    nChunk = Ne;
                else if (z != relz && z == 15)
                    nChunk = Se;
                else
                    nChunk = E;
            else if (z != relz && z == 0)
                nChunk = N;
            else if (z != relz && z == 15)
                nChunk = S;

            if (nChunk == null)
            {
                //happens at current world bounds
                return new Block(BlockType.Rock);
            }
            var block = nChunk.Blocks[x*FlattenOffset + z*Size.Y + rely];
            return block;
        }

        #endregion

        public override string ToString()
        {
            return "chunk at index " + Index;
        }
        
        #region Fields

        private Chunk _mN, _mS, _mE, _mW, _mNe, _mNw, _mSe, _mSw;
            //TODO infinite y would require Top , Bottom, maybe vertical diagonals

        public static Vector3B Size = new Vector3B(16, 128, 16);
        public static Vector3B Max = new Vector3B(15, 127, 15);

        public VertexBuffer VertexBuffer;
        public VertexBuffer WaterVertexBuffer;
        public VertexBuffer GrassVertexBuffer;

        public IndexBuffer IndexBuffer;
        public IndexBuffer WaterIndexBuffer;
        public IndexBuffer GrassIndexBuffer;

        public List<short> IndexList;
        public List<short> WaterIndexList;
        public List<short> GrassIndexList; 

        public List<VertexPositionTextureLight> VertexList;
        public List<VertexPositionTextureLight> WaterVertexList;
        public List<VertexPositionTextureLight> GrassVertexList; 

        public short VertexCount;
        public short WaterVertexCount;
        public short GrassVertexCount;

        /// <summary>
        /// Contains blocks as a flattened array.
        /// </summary>
        public Block[] Blocks;
        /// <summary>
        /// Contains heights as a multidimensional array.
        /// </summary>
        public byte[,] HeightMap;

        /* 
        For accessing array for x,z,y coordianate use the pattern: Blocks[x * Chunk.FlattenOffset + z * Chunk.SIZE.Y + y]
        For allowing sequental access on blocks using iterations, the blocks are stored as [x,z,y]. So basically iterate x first, z then and y last.
        Consider the following pattern;
        for (int x = 0; x < Chunk.WidthInBlocks; x++)
        {
            for (int z = 0; z < Chunk.LenghtInBlocks; z++)
            {
                int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks; // we don't want this x-z value to be calculated each in in y-loop!
                for (int y = 0; y < Chunk.HeightInBlocks; y++)
                {
                    var block=Blocks[offset + y].Id 
        */

        /// <summary>
        /// Used when accessing flatten blocks array.
        /// </summary>
        public static int FlattenOffset = Size.Z*Size.Y;

        public Vector3I Position;
        public Vector3I Index;

        public bool Dirty;
        //public bool visible;
        //public bool generated;
        //public bool built;

        public bool Broken;

        public readonly World World;

        public Vector3B HighestSolidBlock = new Vector3B(0, 0, 0);
        //highestNoneBlock starts at 0 so it will be adjusted. if you start at highest it will never be adjusted ! 

        public Vector3B LowestNoneBlock = new Vector3B(0, Size.Y, 0);

        #endregion

        #region N S E W NE NW SE SW Neighbours accessors

        //this neighbours check can not be done in constructor, there would be some holes => it has to be done at access time 
        //seems there is no mem leak so no need for weakreferences
        public Chunk N
        {
            get
            {
                if (_mN == null) _mN = World.Chunks[Index.X, Index.Z + 1];
                if (_mN != null) _mN._mS = this;
                return _mN;
            }
        }

        public Chunk S
        {
            get
            {
                if (_mS == null) _mS = World.Chunks[Index.X, Index.Z - 1];
                if (_mS != null) _mS._mN = this;
                return _mS;
            }
        }

        public Chunk E
        {
            get
            {
                if (_mE == null) _mE = World.Chunks[Index.X - 1, Index.Z];
                if (_mE != null) _mE._mW = this;
                return _mE;
            }
        }

        public Chunk W
        {
            get
            {
                if (_mW == null) _mW = World.Chunks[Index.X + 1, Index.Z];
                if (_mW != null) _mW._mE = this;
                return _mW;
            }
        }

        public Chunk Nw => _mNw ?? (_mNw = World.Chunks[Index.X + 1, Index.Z + 1]);

        public Chunk Ne => _mNe ?? (_mNe = World.Chunks[Index.X - 1, Index.Z + 1]);

        public Chunk Sw => _mSw ?? (_mSw = World.Chunks[Index.X + 1, Index.Z - 1]);

        public Chunk Se => _mSe ?? (_mSe = World.Chunks[Index.X - 1, Index.Z - 1]);

        public Chunk GetNeighbour(Cardinal c)
        {
            switch (c)
            {
                case Cardinal.N:
                    return N;
                case Cardinal.S:
                    return S;
                case Cardinal.E:
                    return E;
                case Cardinal.W:
                    return W;
                case Cardinal.Se:
                    return Se;
                case Cardinal.Sw:
                    return Sw;
                case Cardinal.Ne:
                    return Ne;
                case Cardinal.Nw:
                    return Nw;
            }
            throw new NotImplementedException();
        }

        #endregion
    }
}