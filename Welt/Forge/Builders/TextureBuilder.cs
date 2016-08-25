﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Welt.API.Forge;
using Welt.Blocks;
using Welt.Types;
using static Welt.Console.ThrowHelper;


namespace Welt.Forge.Builders
{
    /// <summary>
    ///     Builds the UV mappings and stitches the textures together if said textures
    ///     are detached.
    /// </summary>
    public static class TextureBuilder
    {
        private static bool _initialized;
        private static Dictionary<string, Vector2> _textures;
        private static int _textureAtlas;

        private static readonly int[] _textureSizes =
        {
            16, 32, 64, 128, 256, 512, 1048, 2056
        };

        /// <summary>
        ///     Initializes the builder and reads all textures used for the game. Should be called 
        ///     during Game.LoadContent.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) Throw(new InvalidOperationException("Texture builder has already been called"));

            #region Create BlockTextureModels

            // jesus fuck lets try this again. ALRIGHT.
            // step 1: get all the images. 
            

            #endregion


            _initialized = true;
        }

        public static IEnumerable<Vector2> CreateTexture(ushort id, byte md, Mesh mesh)
        {
            var btm = Find(id, md);
            var v = new List<Vector2>();

            if (mesh.IsParentMesh())
            {
                foreach (var submesh in mesh.Submeshes)
                {
                    v.AddRange(CreateTexture(id, md, submesh));
                }
                return v;
            }

            switch (mesh.Face)
            {
                case BlockFaceDirection.XIncreasing:
                    return GetUvMapping(btm.XIncreasingTexture, BlockFaceDirection.XIncreasing);
                case BlockFaceDirection.XDecreasing:
                    return GetUvMapping(btm.XDecreasingTexture, BlockFaceDirection.XDecreasing);
                case BlockFaceDirection.ZIncreasing:
                    return GetUvMapping(btm.ZIncreasingTexture, BlockFaceDirection.ZIncreasing);
                case BlockFaceDirection.ZDecreasing:
                    return GetUvMapping(btm.ZDecreasingTexture, BlockFaceDirection.ZDecreasing);
                case BlockFaceDirection.YIncreasing:
                    return GetUvMapping(btm.YIncreasingTexture, BlockFaceDirection.YIncreasing);
                case BlockFaceDirection.YDecreasing:
                    return GetUvMapping(btm.YDecreasingTexture, BlockFaceDirection.YDecreasing);
                default:
                    throw new ArgumentException("Invalid face supplied for " + nameof(mesh));
            }
        }

        private static IEnumerable<Vector2> GetUvMapping(Vector2 texture, BlockFaceDirection face)
        {
            var ofs = 1f/_textureAtlas;

            var yOfs = texture.Y*ofs;
            var xOfs = texture.X*ofs;
            var uvList = new Vector2[6];

            switch (face)
            {
                case BlockFaceDirection.XIncreasing:
                    uvList[0] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[1] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[2] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[3] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[4] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[5] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    break;
                case BlockFaceDirection.XDecreasing:
                    uvList[0] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[1] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[2] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    uvList[3] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[4] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    uvList[5] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    break;
                case BlockFaceDirection.YIncreasing:
                    uvList[0] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[1] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[2] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[3] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[4] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[5] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    break;
                case BlockFaceDirection.YDecreasing:
                    uvList[0] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[1] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[2] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[3] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[4] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[5] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    break;
                case BlockFaceDirection.ZIncreasing:
                    uvList[0] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[1] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[2] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    uvList[3] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[4] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    uvList[5] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    break;
                case BlockFaceDirection.ZDecreasing:
                    uvList[0] = new Vector2(xOfs, yOfs); // 0,0
                    uvList[1] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[2] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[3] = new Vector2(xOfs, yOfs + ofs); // 0,1
                    uvList[4] = new Vector2(xOfs + ofs, yOfs); // 1,0
                    uvList[5] = new Vector2(xOfs + ofs, yOfs + ofs); // 1,1
                    break;
            }
            return uvList;
        }
    }
}
 