using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace BestBundle
{
    public class BundleEntitiesDatabase
    {
        // 0x0F - Header
        // 0x04 - Entity Infos Count
        // 0x04 - Entity Infos Length
        // ?    - Entity Infos


        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;

        internal BundleEntityInfo[] EntityInfos = new BundleEntityInfo[0];

        private readonly Dictionary<string, int> entitiesNamesMap = new Dictionary<string, int>();
        private readonly HashSet<string> existingEntityTypes = new HashSet<string>();

        internal BundleEntitiesDatabase()
        {

        }

        public string[] GetEntityTypes()
        {
            return existingEntityTypes.ToArray();
        }
        public bool ContainsEntityType(string entityType)
        {
            return existingEntityTypes.Contains(entityType);
        }

        public string GetEntityTypeName(string nameID)
        {
            if(entitiesNamesMap.TryGetValue(nameID, out int id))
            {
                return EntityInfos[id].EntityType;
            }
            return null;
        }
        public Type GetEntityType(string nameId)
        {
            if(entitiesNamesMap.TryGetValue(nameId, out int id))
            {
                string typeName = EntityInfos[id].EntityType;
                return BundleFactory.Instance.GetEntityTypeByTypeName(typeName);
            }
            return null;
        }

        public string[] GetEntityIds()
        {
            return entitiesNamesMap.Keys.ToArray();
        }
        public bool ContainsEntity(string nameId)
        {
            return entitiesNamesMap.ContainsKey(nameId);
        }

        internal bool readEntity(Stream stream, string entityName, out RawEntity entity)
        {
            if (stream.CanSeek && stream.CanRead)
            {
                using (BinaryReader br = new BinaryReader(stream, System.Text.Encoding.UTF8, true))
                {
                    if (entitiesNamesMap.TryGetValue(entityName, out int entityId))
                    {
                        var entityInfo = EntityInfos[entityId];

                        stream.Seek(entityInfo.EntityPosition, SeekOrigin.Begin);

                        entity = new RawEntity();
                        entity.Read(br);
                        return true;
                    }
                }
            }
            entity = new RawEntity();
            return false;
        }

        internal bool Read(BinaryReader reader)
        {
            long firstHeaderPart = reader.ReadInt64();
            long secondHeaderPart = reader.ReadInt64();
            if (firstHeaderPart != HEADER_PART_1 || secondHeaderPart != HEADER_PART_2)
            {
                return false;
            }

            EntityInfos = new BundleEntityInfo[reader.ReadInt32()];

            using (MemoryStream ms = new MemoryStream(reader.ReadBytes(reader.ReadInt32())))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < EntityInfos.Length; i++)
                    {
                        var entity = EntityInfos[i];
                        entity.Read(br);
                        string entityName = entity.NameId;
                        if (!string.IsNullOrEmpty(entityName))
                        {
                            entitiesNamesMap[entityName] = i;
                        }
                        if (!string.IsNullOrEmpty(entity.EntityType))
                        {
                            existingEntityTypes.Add(entity.EntityType);
                        }

                        EntityInfos[i] = entity;
                    }
                }
            }

            return true;
        }
        internal void Write(BinaryWriter writer)
        {
            writer.Write(HEADER_PART_1);
            writer.Write(HEADER_PART_2);

            byte[] binaryEntityInfos = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < EntityInfos.Length; i++)
                    {
                        EntityInfos[i].Write(bw);
                    }
                }

                binaryEntityInfos = ms.ToArray();
            }
            writer.Write(EntityInfos.Length);
            writer.Write(binaryEntityInfos.Length);
            writer.Write(binaryEntityInfos);

        }
    }
}