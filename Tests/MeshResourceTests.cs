using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.IO;
using BestBundle.UnityResources;

namespace BestBundle.Tests
{
    public class MeshResourceTests
    {

        private Mesh originalMesh;

        private byte[] savedBundle;

        private Mesh restoredMesh;

        [OneTimeSetUp]
        [Test]
        public void MeshWriteTest()
        {
            var primitive = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            originalMesh = primitive.GetComponent<MeshFilter>().sharedMesh;

            using (MemoryStream ms = new MemoryStream())
            {
                Assert.IsTrue(BundleFactory.Instance.CreateBundle(ms, "MeshBundle", new Dictionary<string, IResource>() { { "TestMesh", new MeshResource(originalMesh) } }));
                savedBundle = ms.ToArray();
            }
        }

        [Test, Order(0)]
        public void MeshReadTest()
        {
            Assert.IsNotNull(savedBundle);
            Assert.NotZero(savedBundle.Length);

            Debug.Log(savedBundle.Length);

            using (MemoryStream ms = new MemoryStream(savedBundle))
            {
                Assert.IsTrue(Bundle.TryOpenBundle(ms, out Bundle bundle));

                var meshResource = bundle.GetResource<MeshResource>("TestMesh");

                Assert.IsNotNull(meshResource);

                var mesh = meshResource.Mesh;

                Assert.IsNotNull(mesh);

                Assert.AreEqual(mesh.name, originalMesh.name, "Names don't match");

                restoredMesh = mesh;
            }
        }
        [Test, Order(1)]
        public void MeshVertexTrianglesTest()
        {
            Assert.AreEqual(restoredMesh.vertices, originalMesh.vertices, "Vertices do not match");
            Assert.AreEqual(restoredMesh.triangles, originalMesh.triangles, "Polygons do not match");
        }
        [Test, Order(2)]
        public void SubMeshTest()
        {
            Assert.AreEqual(restoredMesh.subMeshCount, originalMesh.subMeshCount);

            for (int i = 0; i < restoredMesh.subMeshCount; i++)
            {
                Assert.AreEqual(restoredMesh.GetSubMesh(i), originalMesh.GetSubMesh(i));
            }
        }
        [Test, Order(3)]
        public void MeshNormalsTangentsTest()
        {
            Assert.AreEqual(restoredMesh.normals, originalMesh.normals, "Normals do not match");

            Assert.AreEqual(restoredMesh.tangents, originalMesh.tangents, "Tangents don't match");
        }
        [Test, Order(4)]
        public void MeshVertexColorsAndUVsTest()
        {
            Assert.AreEqual(restoredMesh.colors32, originalMesh.colors32, "Colors don't match");

            Assert.AreEqual(restoredMesh.uv, originalMesh.uv, "UV1 don't match");
            Assert.AreEqual(restoredMesh.uv2, originalMesh.uv2, "UV2 don't match");
            Assert.AreEqual(restoredMesh.uv3, originalMesh.uv3, "UV3 don't match");
            Assert.AreEqual(restoredMesh.uv4, originalMesh.uv4, "UV4 don't match");
            Assert.AreEqual(restoredMesh.uv5, originalMesh.uv5, "UV5 don't match");
            Assert.AreEqual(restoredMesh.uv6, originalMesh.uv6, "UV6 don't match");
            Assert.AreEqual(restoredMesh.uv7, originalMesh.uv7, "UV7 don't match");
            Assert.AreEqual(restoredMesh.uv8, originalMesh.uv8, "UV8 don't match");
        }
        [Test, Order(5)]
        public void MeshBonesTest()
        {
            var bonePerVertex = restoredMesh.GetBonesPerVertex().ToArray();
            var originalBonePerVertex = originalMesh.GetBonesPerVertex().ToArray();

            Assert.AreEqual(bonePerVertex, originalBonePerVertex);

            var weights = restoredMesh.GetAllBoneWeights().ToArray();
            var originalWeights = originalMesh.GetAllBoneWeights().ToArray();

            Assert.AreEqual(weights.Length, originalWeights.Length);
            for (int i = 0; i < weights.Length; i++)
            {
                Assert.AreEqual(weights[i], originalWeights[i]);
            }
        }
        [Test, Order(6)]
        public void BlendShapesTest()
        {
            Assert.AreEqual(restoredMesh.blendShapeCount, originalMesh.blendShapeCount);

            var blendShapesCount = restoredMesh.blendShapeCount;

            for (int i = 0; i < blendShapesCount; i++)
            {
                Assert.AreEqual(restoredMesh.GetBlendShapeName(i), originalMesh.GetBlendShapeName(i));

                Assert.AreEqual(restoredMesh.GetBlendShapeFrameCount(i), originalMesh.GetBlendShapeFrameCount(i));
                var frameCount = restoredMesh.GetBlendShapeFrameCount(i);

                for (int f = 0; f < frameCount; f++)
                {
                    Assert.AreEqual(restoredMesh.GetBlendShapeFrameWeight(i, f), originalMesh.GetBlendShapeFrameWeight(i, f));

                    var dVertexRestored = new Vector3[restoredMesh.vertexCount];
                    var dNormalsRestored = new Vector3[restoredMesh.vertexCount];
                    var dTangentsRestored = new Vector3[restoredMesh.vertexCount];
                    
                    var dVertexOriginal = new Vector3[restoredMesh.vertexCount];
                    var dNormalsOriginal = new Vector3[restoredMesh.vertexCount];
                    var dTangentsOriginal = new Vector3[restoredMesh.vertexCount];

                    restoredMesh.GetBlendShapeFrameVertices(i, f, dVertexRestored, dNormalsRestored, dTangentsRestored);
                    originalMesh.GetBlendShapeFrameVertices(i, f, dVertexOriginal, dNormalsOriginal, dTangentsOriginal);

                    Assert.AreEqual(dVertexRestored, dVertexOriginal);
                    Assert.AreEqual(dNormalsRestored, dNormalsOriginal);
                    Assert.AreEqual(dTangentsRestored, dTangentsOriginal);
                }
            }
        }
    }
}
