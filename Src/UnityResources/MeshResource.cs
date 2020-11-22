using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BestBundle.UnityResources
{
    public sealed class MeshResource : IResource
    {
        public Mesh Mesh;

        public string ResourceType => "Mesh";

        public MeshResource()
        {

        }
        public MeshResource(Mesh mesh)
        {
            Mesh = mesh;
        }

        public bool Restore(in byte[] rawData)
        {
            if (Mesh == null)
            {
                Mesh = new Mesh();

                using (MemoryStream ms = new MemoryStream(rawData))
                {
                    using (BinaryReader br = new BinaryReader(ms, Encoding.UTF8, true))
                    {
                        if (restoreMeshInfo(br))
                        {
                            if (restoreVertex(br) && restoreTriangles(br))
                            {
                                if (restoreNormals(br) && restoreTangents(br))
                                {
                                    if (restoreColors(br) && restoreUVs(br))
                                    {

                                    }
                                }
                            }
                        }

                    }
                    return true;
                }
            }
            return false;
        }

        public bool Save(out byte[] rawData)
        {
            if (Mesh != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8, true))
                    {
                        if (saveMeshInfo(bw))
                        {
                            if (saveVertex(bw) && saveTriangles(bw))
                            {
                                if (saveNormals(bw) && saveTangents(bw))
                                {
                                    if (saveColors(bw) && saveUVs(bw))
                                    {

                                    }
                                }
                            }
                        }

                    }
                    rawData = ms.ToArray();
                    return true;
                }

            }
            rawData = new byte[0];
            return false;
        }

        #region meshInfo
        private bool saveMeshInfo(BinaryWriter bw)
        {
            bw.Write(Mesh.name);
            return true;
        }
        private bool restoreMeshInfo(BinaryReader br)
        {
            Mesh.name = br.ReadString();
            return true;
        }
        #endregion
        #region Vertex
        private bool saveVertex(BinaryWriter bw)
        {
            var vertices = Mesh.vertices;

            bw.Write(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];

                bw.Write(vertex.x);
                bw.Write(vertex.y);
                bw.Write(vertex.z);
            }
            return true;
        }
        private bool restoreVertex(BinaryReader br)
        {
            var length = br.ReadInt32();

            var vertices = new Vector3[length];

            for (int i = 0; i < length; i++)
            {
                vertices[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            }

            Mesh.vertices = vertices;

            return true;
        }
        #endregion
        #region Triangles
        private bool saveTriangles(BinaryWriter bw)
        {
            var triangles = Mesh.triangles;

            bw.Write(triangles.Length);
            for (int i = 0; i < triangles.Length; i++)
            {
                bw.Write(triangles[i]);
            }
            return true;
        }
        private bool restoreTriangles(BinaryReader br)
        {
            var length = br.ReadInt32();

            var triangles = new int[length];

            for (int i = 0; i < length; i++)
            {
                triangles[i] = br.ReadInt32();
            }

            Mesh.triangles = triangles;
            return true;
        }
        #endregion
        #region Normals
        private bool saveNormals(BinaryWriter bw)
        {
            var normals = Mesh.normals;

            bw.Write(normals.Length);
            for (int i = 0; i < normals.Length; i++)
            {
                var normal = normals[i];

                bw.Write(normal.x);
                bw.Write(normal.y);
                bw.Write(normal.z);
            }
            return true;
        }
        private bool restoreNormals(BinaryReader br)
        {
            var length = br.ReadInt32();

            var normals = new Vector3[length];

            for (int i = 0; i < length; i++)
            {
                normals[i] = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            }

            Mesh.normals = normals;

            return true;
        }
        #endregion
        #region Tangents
        private bool saveTangents(BinaryWriter bw)
        {
            var tangents = Mesh.tangents;

            bw.Write(tangents.Length);
            for (int i = 0; i < tangents.Length; i++)
            {
                var normal = tangents[i];

                bw.Write(normal.x);
                bw.Write(normal.y);
                bw.Write(normal.z);
                bw.Write(normal.w);
            }
            return true;
        }
        private bool restoreTangents(BinaryReader br)
        {
            var length = br.ReadInt32();

            var tangents = new Vector4[length];

            for (int i = 0; i < length; i++)
            {
                tangents[i] = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            }

            Mesh.tangents = tangents;

            return true;
        }
        #endregion
        #region Colors
        private bool saveColors(BinaryWriter bw)
        {
            var colors = Mesh.colors32;

            bw.Write(colors.Length);
            for (int i = 0; i < colors.Length; i++)
            {
                var color = colors[i];

                bw.Write(color.r);
                bw.Write(color.g);
                bw.Write(color.b);
                bw.Write(color.a);
            }
            return true;
        }
        private bool restoreColors(BinaryReader br)
        {
            var length = br.ReadInt32();

            var colors = new Color32[length];

            for (int i = 0; i < length; i++)
            {
                colors[i] = new Color32(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
            }

            Mesh.colors32 = colors;
            return true;
        }
        #endregion

        #region UV's
        private bool saveUVs(BinaryWriter bw)
        {
            var uv1 = Mesh.uv;
            bw.Write(uv1.Length);
            for (int i = 0; i < uv1.Length; i++)
            {
                var coord = uv1[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }

            var uv2 = Mesh.uv2;
            bw.Write(uv2.Length);
            for (int i = 0; i < uv2.Length; i++)
            {
                var coord = uv2[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }

            var uv3 = Mesh.uv3;
            bw.Write(uv3.Length);
            for (int i = 0; i < uv3.Length; i++)
            {
                var coord = uv3[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }

            var uv4 = Mesh.uv4;
            bw.Write(uv4.Length);
            for (int i = 0; i < uv4.Length; i++)
            {
                var coord = uv4[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }

            var uv5 = Mesh.uv5;
            bw.Write(uv5.Length);
            for (int i = 0; i < uv5.Length; i++)
            {
                var coord = uv5[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }

            var uv6 = Mesh.uv6;
            bw.Write(uv6.Length);
            for (int i = 0; i < uv6.Length; i++)
            {
                var coord = uv6[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }

            var uv7 = Mesh.uv7;
            bw.Write(uv7.Length);
            for (int i = 0; i < uv7.Length; i++)
            {
                var coord = uv7[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }

            var uv8 = Mesh.uv8;
            bw.Write(uv8.Length);
            for (int i = 0; i < uv8.Length; i++)
            {
                var coord = uv8[i];
                bw.Write(coord.x);
                bw.Write(coord.y);
            }
            return true;
        }
        private bool restoreUVs(BinaryReader br)
        {
            var uv1Length = br.ReadInt32();
            var uv1 = new Vector2[uv1Length];
            for (int i = 0; i < uv1Length; i++)
            {
                uv1[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv = uv1;

            var uv2Length = br.ReadInt32();
            var uv2 = new Vector2[uv2Length];
            for (int i = 0; i < uv2Length; i++)
            {
                uv2[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv2 = uv2;

            var uv3Length = br.ReadInt32();
            var uv3 = new Vector2[uv3Length];
            for (int i = 0; i < uv3Length; i++)
            {
                uv3[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv3 = uv3;

            var uv4Length = br.ReadInt32();
            var uv4 = new Vector2[uv4Length];
            for (int i = 0; i < uv4Length; i++)
            {
                uv4[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv4 = uv4;

            var uv5Length = br.ReadInt32();
            var uv5 = new Vector2[uv5Length];
            for (int i = 0; i < uv5Length; i++)
            {
                uv5[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv5 = uv5;

            var uv6Length = br.ReadInt32();
            var uv6 = new Vector2[uv6Length];
            for (int i = 0; i < uv6Length; i++)
            {
                uv6[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv6 = uv6;

            var uv7Length = br.ReadInt32();
            var uv7 = new Vector2[uv7Length];
            for (int i = 0; i < uv7Length; i++)
            {
                uv7[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv7 = uv7;

            var uv8Length = br.ReadInt32();
            var uv8 = new Vector2[uv8Length];
            for (int i = 0; i < uv8Length; i++)
            {
                uv8[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
            }
            Mesh.uv8 = uv8;

            return true;
        }
        #endregion

        #region Bones
        #region Bindposes

        #endregion
        #endregion
    }
}
