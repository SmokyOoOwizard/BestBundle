using System.Collections.Generic;
using System;
using System.IO;
using BestBundle.UnityResources;
using System.Threading;

namespace BestBundle
{
    public sealed class BundleFactory
    {
        public static BundleFactory Instance { get; private set; }

        private readonly Dictionary<string, Type> resourcesTypes = new Dictionary<string, Type>();

        internal SynchronizationContext UnitySynchronization;

        public void SetUnitySynchronizationContext(SynchronizationContext context)
        {
            UnitySynchronization = context;
        }

        static BundleFactory()
        {
            Instance = new BundleFactory();

            Instance.AddResourceType<MeshResource>();
        }

        public void AddResourceType<T>() where T : IResource
        {
            var obj = Activator.CreateInstance<T>();
            var resourceType = obj.ResourceType;
            if (!string.IsNullOrEmpty(resourceType))
            {
                resourcesTypes[resourceType] = typeof(T);
            }
        }
        public Type GetResourceTypeByTypeName(string typeName)
        {
            if(resourcesTypes.TryGetValue(typeName, out Type resourceType))
            {
                return resourceType;
            }
            return null;
        }

        internal bool TryRestoreResource(in RawResource raw, out IResource resource)
        {
            if (!string.IsNullOrEmpty(raw.ResourceType))
            {
                if (resourcesTypes.TryGetValue(raw.ResourceType, out Type type))
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj is IResource)
                    {
                        var iresource = obj as IResource;
                        if (iresource.Restore(in raw.Resource))
                        {
                            resource = iresource;
                            return true;
                        }
                    }
                }
            }
            resource = null;
            return false;
        }
        internal bool TryRestoreResource<T>(in RawResource raw, out T resource) where T : IResource
        {
            if (!string.IsNullOrEmpty(raw.ResourceType))
            {
                if (resourcesTypes.TryGetValue(raw.ResourceType, out Type type))
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj is T)
                    {
                        var iresource = (T)obj;
                        if (iresource.Restore(in raw.Resource))
                        {
                            resource = iresource;
                            return true;
                        }
                    }
                }
            }
            resource = default(T);
            return false;
        }

        public bool CreateBundle(Stream output, string name, Dictionary<string, IResource> resources)
        {
            if (!output.CanWrite || !output.CanSeek)
            {
                return false;
            }

            BundleInfo info = new BundleInfo();
            info.Name = name;
            info.CreateTime = DateTime.Now.Ticks;

            using (BinaryWriter bw = new BinaryWriter(output, System.Text.Encoding.UTF8, true))
            {
                info.Write(bw);
                ResourceDatabaseHelper.CreateAndWriteDatabase(output, resources);
            }

            return true;
        }
    }
}