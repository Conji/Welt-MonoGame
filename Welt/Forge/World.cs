﻿#region Copyright
// COPYRIGHT 2015 JUSTIN COX (CONJI)
#endregion
#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Welt.Forge.Generators;
using Welt.Forge.Renderers;
using Welt.Managers;
using Welt.Models;
using Welt.Persistence;
using Welt.Types;

#endregion

namespace Welt.Forge
{
    public class World
    {
        #region choose terrain generation

        //public IChunkGenerator Generator = new SimpleTerrain();
        //public IChunkGenerator Generator = new FlatReferenceTerrain();
        //public IChunkGenerator Generator = new TerrainWithCaves();
        public IChunkGenerator Generator = new DualLayerTerrainWithMediumValleysForRivers();

        // Biomes
        //public IChunkGenerator Generator = new Tundra_Alpine();
        //public IChunkGenerator Generator = new Desert_Subtropical();
        //public IChunkGenerator Generator = new Grassland_Temperate();

        #endregion

        public World(string name)
        {
            //Chunks = new Dictionary2<Chunk>();//
            Chunks = new ChunkManager(new ChunkPersistence());
            Name = name;
        }

        public void ToggleRasterMode()
        {
            Wireframed = !Wireframed;
        }

        #region visitChunks

        public void VisitChunks(Func<Vector3I, Chunk> visitor, byte radius)
        {
            //+1 is for having the player on a center chunk
            for (var x = Origin - radius; x < Origin + radius + 1; x++)
            {
                for (var z = Origin - radius; z < Origin + radius + 1; z++)
                {
                    visitor(new Vector3I(x, 0, z));
                    
                }
            }
        }

        #endregion

        #region InView

        public bool InView(uint x, uint y, uint z)
        {
            if (Chunks[x/Chunk.Size.X, z/Chunk.Size.Z] == null)
                return false;

            var lx = x%Chunk.Size.X;
            var ly = y%Chunk.Size.Y;
            var lz = z%Chunk.Size.Z;

            return lx < Chunk.Size.X && ly < Chunk.Size.Y && lz < Chunk.Size.Z;
        }

        #endregion

        #region Fields

        public ChunkManager Chunks;

        //public const byte VIEW_CHUNKS_X = 8;
        //public const byte VIEW_CHUNKS_Y = 1;
        //public const byte VIEW_CHUNKS_Z = 8;

        public string Name { get; }

        public static int Seed = 54321;

        public static uint Origin = 1000;
        //TODO UInt32 requires decoupling rendering coordinates to avoid float problems

        //public const byte VIEW_DISTANCE_NEAR_X = VIEW_CHUNKS_X * 2;
        // public const byte VIEW_DISTANCE_NEAR_Z = VIEW_CHUNKS_Z * 2;

        //public const byte VIEW_DISTANCE_FAR_X = VIEW_CHUNKS_X * 4;
        //public const byte VIEW_DISTANCE_FAR_Z = VIEW_CHUNKS_Z * 4;

        public readonly RasterizerState WireframedRaster = new RasterizerState
        {
            CullMode = CullMode.None,
            FillMode = FillMode.WireFrame
        };

        public readonly RasterizerState NormalRaster = new RasterizerState
        {
            CullMode = CullMode.CullCounterClockwiseFace,
            FillMode = FillMode.Solid
        };

        public bool Wireframed;

        // Day/Night
        public float Tod = 12; // Midday
        public Vector3 SunPos = new Vector3(0, 1, 0); // Directly overhead
        public bool RealTime = false;
        public bool DayMode = false;
        public bool NightMode = false;

        internal SimpleRenderer Renderer;

        #region Atmospheric settings

        public Vector4 Nightcolor = Color.Red.ToVector4();
        public Vector4 Suncolor = Color.White.ToVector4();
        public Vector4 Horizoncolor = Color.White.ToVector4();

        public Vector4 Eveningtint = Color.Red.ToVector4();
        public Vector4 Morningtint = Color.Gold.ToVector4();

        //private float _tod;
        //public bool dayMode = false;
        //public bool nightMode = false;
        public float Fognear = 14*16; //(BUILD_RANGE - 1) * 16;
        public float Fogfar = 16*16; //(BUILD_RANGE + 1) * 16;

        #endregion

        #endregion

        #region GetBlock

        public Block GetBlock(Vector3 position)
        {
            return GetBlock((uint) position.X, (uint) position.Y, (uint) position.Z);
        }

        public Chunk ChunkAt(Vector3 position)
        {
            var x = (uint) position.X;
            var z = (uint) position.Z;

            var cx = x/Chunk.Size.X;
            var cz = z/Chunk.Size.Z;

            var at = Chunks[cx, cz];

            return at;
        }

        public Block GetBlock(uint x, uint y, uint z)
        {
            if (!InView(x, y, z))
                return new Block(BlockType.None);
            //TODO blocktype.unknown ( with matrix films green symbols texture ? ) 
            var chunk = Chunks[x/Chunk.Size.X, z/Chunk.Size.Z];
            return chunk.Blocks[x%Chunk.Size.X*Chunk.FlattenOffset + z%Chunk.Size.Z*Chunk.Size.Y + y%Chunk.Size.Y];
            //Debug.WriteLine("no block at  ({0},{1},{2}) ", x, y, z);
        }

        #endregion

        #region SetBlock

        public Block SetBlock(Vector3I pos, Block b)
        {
            var result = SetBlock(pos.X, pos.Y, pos.Z, b);
            return result.HasError ? new Block() : result.Value;
        }

        public Maybe<Block, Exception> SetBlock(uint x, uint y, uint z, Block newType)
        {
            if (!InView(x, y, z))
                return new Maybe<Block, Exception>(new Block(),
                    new AccessViolationException("Cannot access block at this distance."));
            var chunk = Chunks[x/Chunk.Size.X, z/Chunk.Size.Z];

            var localX = (byte) (x%Chunk.Size.X);
            var localY = (byte) (y%Chunk.Size.Y);
            var localZ = (byte) (z%Chunk.Size.Z);

            var m = Maybe<Block, Exception>.Check(() =>
            {
                var old = chunk.Blocks[localX*Chunk.FlattenOffset + localZ*Chunk.Size.Y + localY];

                //chunk.SetBlock is also called by terrain generators for Y loops min max optimisation
                chunk.SetBlock(localX, localY, localZ, new Block(newType.Id));

                //Chunk should be responsible for maintaining this
                chunk.State = ChunkState.AwaitingRelighting;

                // use Chunk accessors
                if (localX == 0)
                {
                    if (chunk.E != null) chunk.E.State = ChunkState.AwaitingRelighting;
                }
                if (localX == Chunk.Max.X)
                {
                    //viewableChunks[(x / Chunk.SIZE.X) + 1, z / Chunk.SIZE.Z].dirty = true;
                    if (chunk.W != null) chunk.W.State = ChunkState.AwaitingRelighting;
                }
                if (localZ == 0)
                {
                    //viewableChunks[x / Chunk.SIZE.X, (z / Chunk.SIZE.Z) - 1].dirty = true;
                    if (chunk.S != null) chunk.S.State = ChunkState.AwaitingRelighting;
                }
                if (localZ == Chunk.Max.Z)
                {
                    //viewableChunks[x / Chunk.SIZE.X, (z / Chunk.SIZE.Z) + 1].dirty = true;
                    if (chunk.N != null) chunk.N.State = ChunkState.AwaitingRelighting;
                }
                return old;
            });

            return m;
        }

        #endregion

        public void SetLightAt(uint x, uint y, uint z, Vector3B light)
        {
            var oldBlock = GetBlock(x, y, z);
            oldBlock.R = light.X;
            oldBlock.G = light.Y;
            oldBlock.B = light.Z;
            SetBlock(x, y, z, oldBlock);
            Renderer.DoLightFor(ChunkAt(new Vector3(x, y, z)));
        }
    }
}