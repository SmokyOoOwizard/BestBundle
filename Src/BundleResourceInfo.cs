using System;
using System.IO;
using System.Text;

namespace BestBundle
{
    internal struct BundleResourceInfo
    {
        // 0x04 - Local Id
        // ?    - Name Id
        // ?    - Resource Type
        // 0x08 - Resource Length
        // 0x08 - Resource Position

        public int LocalId;
        public string NameId;
        public string ResourceType;
        public long ResourceLength;
        public long ResourcePosition;


        public bool Read(BinaryReader reader)
        {
            LocalId = reader.ReadInt32();

            NameId = reader.ReadString();
            ResourceType = reader.ReadString();

            ResourceLength = reader.ReadInt64();
            ResourcePosition = reader.ReadInt64();

            return true;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(LocalId);

            writer.Write(NameId);
            writer.Write(ResourceType);

            writer.Write(ResourceLength);
            writer.Write(ResourcePosition);
        }
    }
}