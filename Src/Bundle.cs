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
        // ?    - Resources Info Database
        // ?    - Resources

        public BundleInfo Info { get { return info; } }
        private BundleInfo info;

        private BundleResourceDatabase resourceDatabase = new BundleResourceDatabase();

        private Stream bundleAccessReadStream;

        internal Bundle()
        {

        }

        internal Bundle(string name)
        {
            info.Name = name;
            info.CreateTime = DateTime.Now.Ticks;
        }

        public IResource GetResource(string name)
        {
            if(resourceDatabase.readResource(bundleAccessReadStream, name, out RawResource rawResource))
            {
                if(BundleFactory.Instance.TryRestoreResource(in rawResource, out IResource resource))
                {
                    return resource;
                }
            }
            return null;
        }
        public T GetResource<T>(string name) where T : IResource
        {
            if(resourceDatabase.readResource(bundleAccessReadStream, name, out RawResource rawResource))
            {
                if(BundleFactory.Instance.TryRestoreResource<T>(in rawResource, out T resource))
                {
                    return resource;
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
                    return resourceDatabase.Read(br);
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