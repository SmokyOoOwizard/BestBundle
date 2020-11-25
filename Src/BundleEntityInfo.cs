using System;
using System.IO;
using System.Text;

namespace BestBundle
{
    internal struct BundleEntityInfo
    {
        // 0x04 - Local Id
        // ?    - Name Id
        // ?    - Entity Type
        // 0x08 - Entity Length
        // 0x08 - Entity Position

        public int LocalId;
        public string NameId;
        public string EntityType;
        public long EntityLength;
        public long EntityPosition;


        public bool Read(BinaryReader reader)
        {
            LocalId = reader.ReadInt32();

            NameId = reader.ReadString();
            EntityType = reader.ReadString();

            EntityLength = reader.ReadInt64();
            EntityPosition = reader.ReadInt64();

            return true;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(LocalId);

            writer.Write(NameId);
            writer.Write(EntityType);

            writer.Write(EntityLength);
            writer.Write(EntityPosition);
        }
    }
}