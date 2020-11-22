using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.IO;

namespace BestBundle.Tests
{
    public class BundleReadWriteTests
    {
        public class TestResource : IResource
        {
            public string ResourceType => "TestResource";

            public byte[] SavedBytes = new byte[] { 0xFF, 0x0F, 0x86, 0, 0, 0, 0, 0x25 };
            public byte[] RestoredBytes;

            public bool Restore(in byte[] rawData)
            {
                RestoredBytes = rawData;
                return true;
            }

            public bool Save(out byte[] rawData)
            {
                rawData = SavedBytes;
                return true;
            }
        }
        public class TestResource2 : IResource
        {
            public string ResourceType => "TestResour2";

            public byte[] SavedBytes = new byte[] { 0xFF, 0x0F, 0x86, 0, 0, 1, 0, 0x25 };
            public byte[] RestoredBytes;

            public bool Restore(in byte[] rawData)
            {
                RestoredBytes = rawData;
                return true;
            }

            public bool Save(out byte[] rawData)
            {
                rawData = SavedBytes;
                return true;
            }
        }

        MemoryStream ms;

        [OneTimeSetUp]
        [Test]
        public void BundleWriteTest()
        {
            BundleFactory.Instance.AddResourceType<TestResource>();
            BundleFactory.Instance.AddResourceType<TestResource2>();

            ms = new MemoryStream();

            BundleFactory.Instance.CreateBundle(ms, "Test Bundle", new Dictionary<string, IResource>() { { "Test", new TestResource() }, { "2Test", new TestResource2() } });

            var binary = ms.ToArray();
            Assert.IsNotNull(binary);
            Assert.NotZero(binary.Length);
        }

        [Test]
        public void BundleReadTest()
        {
            ms.Seek(0, SeekOrigin.Begin);

            Assert.IsTrue(Bundle.TryOpenBundle(ms, out Bundle localBundle));

            Assert.AreEqual("Test Bundle", localBundle.Info.Name);

            Assert.IsTrue(localBundle.ResourceDatabase.ContainsResourceType("TestResource"));

            Assert.IsTrue(localBundle.ResourceDatabase.ContainsResource("Test"));

            var resource1 = localBundle.GetResource("Test");

            Assert.IsNotNull(resource1);
            Assert.IsTrue(resource1 is TestResource);
            Assert.AreEqual((resource1 as TestResource).SavedBytes, (resource1 as TestResource).RestoredBytes);

            var resource2 = localBundle.GetResource<TestResource2>("2Test");

            Assert.IsNotNull(resource2);
            Assert.AreEqual(resource2.SavedBytes, resource2.RestoredBytes);

            var resource3 = localBundle.GetResource("Null");
            Assert.IsNull(resource3);

            var resource4 = localBundle.GetResource<TestResource2>("Test");
            Assert.IsNull(resource4);

        }

        ~BundleReadWriteTests()
        {
            if (ms != null)
            {
                //ms.Close();
            }
        }
    }
}
