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

        private readonly Dictionary<string, Type> entityTypes = new Dictionary<string, Type>();

        internal SynchronizationContext UnitySynchronization;

        public void SetUnitySynchronizationContext(SynchronizationContext context)
        {
            UnitySynchronization = context;
        }

        static BundleFactory()
        {
            Instance = new BundleFactory();

            Instance.AddEntityType<MeshEntity>();
        }

        public void AddEntityType<T>() where T : IBundleEntity
        {
            var obj = Activator.CreateInstance<T>();
            var entityType = obj.EntityType;
            if (!string.IsNullOrEmpty(entityType))
            {
                entityTypes[entityType] = typeof(T);
            }
        }
        public Type GetEntityTypeByTypeName(string typeName)
        {
            if(entityTypes.TryGetValue(typeName, out Type entityType))
            {
                return entityType;
            }
            return null;
        }

        internal bool TryRestoreEntity(in RawEntity raw, out IBundleEntity entity)
        {
            if (!string.IsNullOrEmpty(raw.EntityType))
            {
                if (entityTypes.TryGetValue(raw.EntityType, out Type type))
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj is IBundleEntity)
                    {
                        var emptyEntity = obj as IBundleEntity;
                        if (emptyEntity.Restore(in raw.Entity))
                        {
                            entity = emptyEntity;
                            return true;
                        }
                    }
                }
            }
            entity = null;
            return false;
        }
        internal bool TryRestoreEntity<T>(in RawEntity raw, out T entity) where T : IBundleEntity
        {
            if (!string.IsNullOrEmpty(raw.EntityType))
            {
                if (entityTypes.TryGetValue(raw.EntityType, out Type type))
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj is T)
                    {
                        var emptyEntity = (T)obj;
                        if (emptyEntity.Restore(in raw.Entity))
                        {
                            entity = emptyEntity;
                            return true;
                        }
                    }
                }
            }
            entity = default(T);
            return false;
        }

        public bool CreateBundle(Stream output, string name, Dictionary<string, IBundleEntity> entities)
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
                EntityDatabaseHelper.CreateAndWriteDatabase(output, entities);
            }

            return true;
        }
    }
}