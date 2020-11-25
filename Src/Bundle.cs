using System.Collections;
using UnityEngine;
using System;
using System.IO;

namespace BestBundle
{
    public class Bundle
    {
        // 0x0F - Header
        // ?    - Bundle Info
        // ?    - Entity Info Database
        // ?    - Entity

        public BundleInfo Info { get { return info; } }
        private BundleInfo info;

        public BundleEntitiesDatabase EntityDatabase { get; private set; } = new BundleEntitiesDatabase();

        private Stream bundleAccessReadStream;

        private object getEntityLockObj = new object();

        internal Bundle()
        {

        }

        internal Bundle(string name)
        {
            info.Name = name;
            info.CreateTime = DateTime.Now.Ticks;
        }

        public IBundleEntity GetEntity(string name)
        {
            lock (getEntityLockObj)
            {
                if (EntityDatabase.readEntity(bundleAccessReadStream, name, out RawEntity rawEntity))
                {
                    if (BundleFactory.Instance.TryRestoreEntity(in rawEntity, out IBundleEntity entity))
                    {
                        return entity;
                    }
                }
            }
            return null;
        }
        public T GetEntity<T>(string name) where T : IBundleEntity
        {
            lock (getEntityLockObj)
            {
                if (EntityDatabase.readEntity(bundleAccessReadStream, name, out RawEntity rawEntity))
                {
                    if (BundleFactory.Instance.TryRestoreEntity<T>(in rawEntity, out T entity))
                    {
                        return entity;
                    }
                }
            }
            return default(T);
        }

        private bool changeBundleAccessReadStream(Stream stream)
        {
            if (stream.CanRead && stream.CanSeek)
            {
                bundleAccessReadStream = stream;
                return true;
            }
            return false;
        }
        private bool readBundleInfoAndDatabase()
        {
            using (BinaryReader br = new BinaryReader(bundleAccessReadStream, System.Text.Encoding.UTF8, true))
            {
                if (info.Read(br))
                {
                    return EntityDatabase.Read(br);
                }
            }
            return false;
        }

        public static bool TryOpenBundle(Stream stream, out Bundle bundle)
        {
            Bundle b = new Bundle();
            if (b.changeBundleAccessReadStream(stream))
            {
                if (b.readBundleInfoAndDatabase())
                {
                    bundle = b;
                    return true;
                }
            }
            bundle = null;
            return false;
        }
    }
}