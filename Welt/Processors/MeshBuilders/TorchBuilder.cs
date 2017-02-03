﻿using Microsoft.Xna.Framework;
using Welt.Forge;
using Welt.API;
using Welt.Core.Forge;
using Welt.API.Forge;
using Welt.Graphics;
using System.Collections.Generic;
using Welt.Forge.Renderers;

namespace Welt.Processors.MeshBuilders
{
    public class TorchBuilder : BlockMeshBuilder
    {
        public static void BuildPostVertexList(IBlockProvider provider, ReadOnlyChunk chunk, Vector3I chunkRelativePosition,
        ChunkRenderer.VisibleFaces faces, int vertexCount,
        ref List<VertexPositionTextureLightEffect> vertices, ref List<short> indices)
        {

            var blockPosition = chunk.GetPosition() + chunkRelativePosition;
            
            BuildPostVertices(chunk, blockPosition, chunkRelativePosition, provider, 0.6f, Color.LightGray, vertexCount,
                ref vertices, ref indices);
        }

        public const int VertexCount = 32;

        private static void BuildPostVertices(ReadOnlyChunk chunk, Vector3I blockPosition, 
            Vector3I chunkRelativePosition, IBlockProvider provider, 
            float sunLight, Color localLight, int vertexCount,
            ref List<VertexPositionTextureLightEffect> vertices, ref List<short> indices)
        {
            var sun = new[] { sunLight, sunLight, sunLight, sunLight };
            var local = new[] { localLight, localLight, localLight, localLight };
            var uvList = provider.GetTexture(BlockFaceDirection.XIncreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(0.55f, 1, 1), new Vector3(0.55f, 1, 0), new Vector3(0.55f, 0, 1), new Vector3(0.55f, 0, 0) },
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);

            uvList = provider.GetTexture(BlockFaceDirection.XDecreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(0.55f, 1, 0), new Vector3(0.55f, 1, 1), new Vector3(0.55f, 0, 0), new Vector3(0.55f, 0, 1) },
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);

            uvList = provider.GetTexture(BlockFaceDirection.XIncreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(0.45f, 1, 1), new Vector3(0.45f, 1, 0), new Vector3(0.45f, 0, 1), new Vector3(0.45f, 0, 0) },
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);
          
            uvList = provider.GetTexture(BlockFaceDirection.XDecreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(0.45f, 1, 0), new Vector3(0.45f, 1, 1), new Vector3(0.45f, 0, 0), new Vector3(0.45f, 0, 1) },
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);

            uvList = provider.GetTexture(BlockFaceDirection.ZIncreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(0, 1, 0.55f), new Vector3(1, 1, 0.55f), new Vector3(0, 0, 0.55f), new Vector3(1, 0, 0.55f) },
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);

            uvList = provider.GetTexture(BlockFaceDirection.ZDecreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(1, 1, 0.55f), new Vector3(0, 1, 0.55f), new Vector3(1, 0, 0.55f), new Vector3(0, 0, 0.55f) },
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);

            uvList = provider.GetTexture(BlockFaceDirection.ZIncreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(0, 1, 0.45f), new Vector3(1, 1, 0.45f), new Vector3(0, 0, 0.45f), new Vector3(1, 0, 0.45f) },
                new Vector2[] { uvList[0], uvList[1], uvList[5], uvList[2] },
                new short[] { 0, 1, 3, 0, 3, 2 }, vertexCount, ref vertices, ref indices);

            uvList = provider.GetTexture(BlockFaceDirection.ZDecreasing);
            RenderMesh(provider, blockPosition, sun, local,
                new Vector3[] { new Vector3(1, 1, 0.45f), new Vector3(0, 1, 0.45f), new Vector3(1, 0, 0.45f), new Vector3(0, 0, 0.45f) },
                new Vector2[] { uvList[0], uvList[1], uvList[2], uvList[5] },
                new short[] { 0, 1, 2, 2, 1, 3 }, vertexCount, ref vertices, ref indices);
            
            //uvList = TextureHelper.UvMappings[(int)texture * 6 + (int)BlockFaceDirection.YIncreasing];
            //AddPlane(chunk, blockType, blockPosition, chunkRelativePosition, BlockFaceDirection.YIncreasing,
            //    new float[] { sunLight, sunLight, sunLight, sunLight },
            //    new Color[] { localLight, localLight, localLight, localLight },
            //    new Vector3[] { new Vector3(0.55f, 0.45f, 0.55f), new Vector3(0.45f, 0.45f, 0.55f), new Vector3(0.55f, 0.45f, 0.45f), new Vector3(0.45f, 0.45f, 0.45f) },
            //    new Vector2[] { uvList[4], uvList[5], uvList[1], uvList[3] },
            //    new short[] { 3, 2, 0, 3, 0, 1 });
        }
    }
}
