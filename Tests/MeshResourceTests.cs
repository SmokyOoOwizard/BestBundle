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

        [OneTimeSetUp]
        [Test]
        public void MeshWriteTest()
        {
            BundleFactory.Instance.AddResourceType<MeshResource>();

            var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            originalMesh = primitive.GetComponent<MeshFilter>().sharedMesh;

            using(MemoryStream ms = new MemoryStream())
            {
                Assert.IsTrue(BundleFactory.Instance.CreateBundle(ms, "MeshBundle", new Dictionary<string, IResource>() { { "TestMesh", new MeshResource(originalMesh) } }));
                savedBundle = ms.ToArray();
            }
        }

        [Test]
        public void MeshReadTest()
        {
            Assert.IsNotNull(savedBundle);
            Assert.NotZero(savedBundle.Length);

            using(MemoryStream ms = new MemoryStream(savedBundle))
            {
                Assert.IsTrue(Bundle.TryOpenBundle(ms, out Bundle bundle));

                var meshResource = bundle.GetResource<MeshResource>("TestMesh");

                Assert.IsNotNull(meshResource);

                var mesh = meshResource.Mesh;

                Assert.IsNotNull(mesh);

                Assert.AreEqual(mesh.name, originalMesh.name, "Names don't match");

                Assert.AreEqual(mesh.vertices, originalMesh.vertices, "Vertices do not match");

                Assert.AreEqual(mesh.triangles, originalMesh.triangles, "Polygons do not match");

                Assert.AreEqual(mesh.normals, originalMesh.normals, "Normals do not match");

                Assert.AreEqual(mesh.tangents, originalMesh.tangents, "Tangents don't match");

                Assert.AreEqual(mesh.colors32, originalMesh.colors32, "Colors don't match");

                Assert.AreEqual(mesh.uv, originalMesh.uv, "UV1 don't match");
                Assert.AreEqual(mesh.uv2, originalMesh.uv2, "UV2 don't match");
                Assert.AreEqual(mesh.uv3, originalMesh.uv3, "UV3 don't match");
                Assert.AreEqual(mesh.uv4, originalMesh.uv4, "UV4 don't match");
                Assert.AreEqual(mesh.uv5, originalMesh.uv5, "UV5 don't match");
                Assert.AreEqual(mesh.uv6, originalMesh.uv6, "UV6 don't match");
                Assert.AreEqual(mesh.uv7, originalMesh.uv7, "UV7 don't match");
                Assert.AreEqual(mesh.uv8, originalMesh.uv8, "UV8 don't match");
            }
        }
    }
}
