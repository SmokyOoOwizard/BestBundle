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
        public class TestEntity : IBundleEntity
        {
            public string EntityType => "TestEntity";

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
        public class TestEntity2 : IBundleEntity
        {
            public string EntityType => "TestEntity2";

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

        byte[] savedBundle;

        [OneTimeSetUp]
        [Test]
        public void BundleWriteTest()
        {
            BundleFactory.Instance.AddEntityType<TestEntity>();
            BundleFactory.Instance.AddEntityType<TestEntity2>();

            using (MemoryStream ms = new MemoryStream())
            {

                BundleFactory.Instance.CreateBundle(ms, "Test Bundle", new Dictionary<string, IBundleEntity>() { { "Test", new TestEntity() }, { "2Test", new TestEntity2() } });

                var binary = ms.ToArray();
                Assert.IsNotNull(binary);
                Assert.NotZero(binary.Length);

                savedBundle = binary;
            }
        }

        [Test]
        public void BundleReadTest()
        {
            using (MemoryStream ms = new MemoryStream(savedBundle))
            {
                Assert.IsTrue(Bundle.TryOpenBundle(ms, out Bundle localBundle));

                Assert.AreEqual("Test Bundle", localBundle.Info.Name);

                Assert.IsTrue(localBundle.EntityDatabase.ContainsEntityType("TestEntity"));

                Assert.IsTrue(localBundle.EntityDatabase.ContainsEntity("Test"));

                var entity1 = localBundle.GetEntity("Test");

                Assert.IsNotNull(entity1);
                Assert.IsTrue(entity1 is TestEntity);
                Assert.AreEqual((entity1 as TestEntity).SavedBytes, (entity1 as TestEntity).RestoredBytes);

                var entity2 = localBundle.GetEntity<TestEntity2>("2Test");

                Assert.IsNotNull(entity2);
                Assert.AreEqual(entity2.SavedBytes, entity2.RestoredBytes);

                var entity3 = localBundle.GetEntity("Null");
                Assert.IsNull(entity3);

                var entity4 = localBundle.GetEntity<TestEntity2>("Test");
                Assert.IsNull(entity4);
            }
        }
    }
}
