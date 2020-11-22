using System;
using System.IO;

namespace BestBundle
{
    internal struct RawResource
    {
        // 0x0F - Header
        // ?    - Resource Type
        // 0x04 - Local Id
        // 0x04 - Length
        // ?    - Resource

        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;
        public int LocalId;
        public string ResourceType;
        public int ResourceLength;
        public byte[] Resource;

        public bool Read(BinaryReader reader)
        {
            long firstHeaderPart = reader.ReadInt64();
            long secondHeaderPart = reader.ReadInt64();
            if (firstHeaderPart != HEADER_PART_1 || secondHeaderPart != HEADER_PART_2)
            {
                return false;
            }

            LocalId = reader.ReadInt32();

            ResourceType = reader.ReadString();

            ResourceLength = reader.ReadInt32();
            Resource = reader.ReadBytes(ResourceLength);

            return true;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(HEADER_PART_1);
            writer.Write(HEADER_PART_2);
            writer.Write(LocalId);

            writer.Write(ResourceType);

            writer.Write(ResourceLength);
            writer.Write(Resource);
        }
    }
}