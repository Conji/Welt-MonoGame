﻿using Microsoft.Xna.Framework;
using Welt.Forge;
using Welt.API;
using Welt.Forge.Renderers;
using Welt.Graphics;
using System.Collections.Generic;
using Welt.API.Forge;

namespace Welt.Processors.MeshBuilders
{
    public class GrassBuilder : BlockMeshBuilder
    {
        public static void BuildBlockVertexList(IBlockProvider provider, ReadOnlyChunk chunk,
            Vector3I chunkRelativePosition, BlockFaceDirection face, int vertexCount,
            ref List<VertexPositionNormalTextureEffect> vertices, ref List<short> indices)
        {

            var blockPosition = chunk.GetPosition() + chunkRelativePosition;

            //get signed bytes from these to be able to remove 1 without further casts
            var x = (sbyte)chunkRelativePosition.X;
            var y = (sbyte)chunkRelativePosition.Y;
            var z = (sbyte)chunkRelativePosition.Z;

            BuildGrassVertices(chunk, blockPosition, chunkRelativePosition, provider, vertexCount, ref vertices, ref indices);
        }

        protected static void BuildGrassVertices(ReadOnlyChunk chunk, Vector3I blockPosition,
            Vector3I chunkRelativePosition, IBlockProvider provider, int vertexCount,
            ref List<VertexPositionNormalTextureEffect> vertices, ref List<short> indices)
        {
            var uvList = provider.GetTexture(BlockFaceDirection.XIncreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(0.3f, 1, 1), new Vector3(0.3f, 1, 0), new Vector3(0.3f, 0, 1), new Vector3(0.3f, 0, 0) },
                Normals[(int)BlockFaceDirection.XIncreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);

            uvList = provider.GetTexture(BlockFaceDirection.XDecreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(0.3f, 1, 0), new Vector3(0.3f, 1, 1), new Vector3(0.3f, 0, 0), new Vector3(0.3f, 0, 1) },
                Normals[(int)BlockFaceDirection.XDecreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);
            
            uvList = provider.GetTexture(BlockFaceDirection.XIncreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(0.7f, 1, 1), new Vector3(0.7f, 1, 0), new Vector3(0.7f, 0, 1), new Vector3(0.7f, 0, 0) },
                Normals[(int)BlockFaceDirection.XIncreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);
            
            uvList = provider.GetTexture(BlockFaceDirection.XDecreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(0.7f, 1, 0), new Vector3(0.7f, 1, 1), new Vector3(0.7f, 0, 0), new Vector3(0.7f, 0, 1) },
                Normals[(int)BlockFaceDirection.XDecreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);
            
            uvList = provider.GetTexture(BlockFaceDirection.ZIncreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(0, 1, 0.3f), new Vector3(1, 1, 0.3f), new Vector3(0, 0, 0.3f), new Vector3(1, 0, 0.3f) },
                Normals[(int)BlockFaceDirection.ZIncreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);
            
            uvList = provider.GetTexture(BlockFaceDirection.ZDecreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(1, 1, 0.3f), new Vector3(0, 1, 0.3f), new Vector3(1, 0, 0.3f), new Vector3(0, 0, 0.3f) },
                Normals[(int)BlockFaceDirection.ZDecreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);
            
            uvList = provider.GetTexture(BlockFaceDirection.ZIncreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(0, 1, 0.7f), new Vector3(1, 1, 0.7f), new Vector3(0, 0, 0.7f), new Vector3(1, 0, 0.7f) },
                Normals[(int)BlockFaceDirection.ZIncreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);
            
            uvList = provider.GetTexture(BlockFaceDirection.ZDecreasing);
            RenderMesh(provider, blockPosition,
                new Vector3[] { new Vector3(1, 1, 0.7f), new Vector3(0, 1, 0.7f), new Vector3(1, 0, 0.7f), new Vector3(0, 0, 0.7f) },
                Normals[(int)BlockFaceDirection.ZDecreasing],
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);
            
        }
    }
}
