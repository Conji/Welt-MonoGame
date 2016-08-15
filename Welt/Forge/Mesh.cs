﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Welt.API.Forge;
using Welt.Blocks;

namespace Welt.Forge
{
    public sealed class Mesh
    {
        public static int VertexCount { get; private set; }
        public static int IndexCount { get; private set; }

        public readonly Vector3[] Vertices;
        public readonly byte[] Indices;
        public readonly BlockFaceDirection Face;

        public readonly List<Mesh> Submeshes;

        /// <summary>
        ///     Creates a parent mesh with empty vertices and indices.
        /// </summary>
        public Mesh() : this(new Vector3[0], new byte[0], BlockFaceDirection.None)
        {
            
        }

        /// <summary>
        ///     Creates a child mesh with assigned vertices and indices.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="face"></param>
        /// <param name="i"></param>
        public Mesh(IEnumerable<Vector3> v, IEnumerable<byte> i, BlockFaceDirection face)
        {
            Vertices = v.ToArray();
            Indices = i.ToArray();
            Face = face;
            Submeshes = new List<Mesh>(16);
            VertexCount += Vertices.Length;
            IndexCount += Indices.Length;
        }

        public void AddSubmesh(Mesh mesh)
        {
            Submeshes.Add(mesh);
        }

        public bool IsParentMesh()
        {
            return Submeshes.Any();
        }

        ~Mesh()
        {
            VertexCount -= Vertices.Length;
            IndexCount -= Indices.Length;
            Submeshes.Clear();
        }
    }
}