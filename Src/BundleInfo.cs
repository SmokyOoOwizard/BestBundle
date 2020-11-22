using System;
using System.IO;
using System.Text;

namespace BestBundle
{
    public struct BundleInfo
    {
        // 0x0F - Header
        // ?    - Name
        // 0x08 - Bundle Create Time

        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;

        public string Name;
        public long CreateTime;


        public bool Read(BinaryReader reader)
        {
            try
            {
                long firstHeaderPart = reader.ReadInt64();
                long secondHeaderPart = reader.ReadInt64();
                if (firstHeaderPart != HEADER_PART_1 || secondHeaderPart != HEADER_PART_2)
                {
                    return false;
                }

                Name = reader.ReadString();

                CreateTime = reader.ReadInt64();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(HEADER_PART_1);
            writer.Write(HEADER_PART_2);

            writer.Write(Name);

            writer.Write(CreateTime);
        }
    }
}