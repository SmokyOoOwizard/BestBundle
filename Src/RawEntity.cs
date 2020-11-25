using System;
using System.IO;

namespace BestBundle
{
    internal struct RawEntity
    {
        // 0x0F - Header
        // ?    - Entity Type
        // 0x04 - Local Id
        // 0x04 - Length
        // ?    - Entity

        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;
        public int LocalId;
        public string EntityType;
        public int EntityLength;
        public byte[] Entity;

        public bool Read(BinaryReader reader)
        {
            long firstHeaderPart = reader.ReadInt64();
            long secondHeaderPart = reader.ReadInt64();
            if (firstHeaderPart != HEADER_PART_1 || secondHeaderPart != HEADER_PART_2)
            {
                return false;
            }

            LocalId = reader.ReadInt32();

            EntityType = reader.ReadString();

            EntityLength = reader.ReadInt32();
            Entity = reader.ReadBytes(EntityLength);

            return true;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(HEADER_PART_1);
            writer.Write(HEADER_PART_2);
            writer.Write(LocalId);

            writer.Write(EntityType);

            writer.Write(EntityLength);
            writer.Write(Entity);
        }
    }
}