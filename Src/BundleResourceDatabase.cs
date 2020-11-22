using System;
using System.IO;
using System.Collections.Generic;

namespace BestBundle
{
    internal class BundleResourceDatabase
    {
        // 0x0F - Header
        // 0x04 - Resource Infos Count
        // 0x04 - Resource Infos Length
        // ?    - Resource Infos


        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;

        internal BundleResourceInfo[] ResourceInfos = new BundleResourceInfo[0];

        private readonly Dictionary<string, int> ResourcesNamesMap = new Dictionary<string, int>();

        internal bool readResource(Stream stream, string resourceName, out RawResource resource)
        {
            if (stream.CanSeek && stream.CanRead)
            {
                using (BinaryReader br = new BinaryReader(stream, System.Text.Encoding.UTF8, true))
                {
                    if (ResourcesNamesMap.TryGetValue(resourceName, out int resId))
                    {
                        var resInfo = ResourceInfos[resId];

                        stream.Seek(resInfo.ResourcePosition, SeekOrigin.Begin);

                        resource = new RawResource();
                        resource.Read(br);
                        return true;
                    }
                }
            }
            resource = new RawResource();
            return false;
        }

        public bool Read(BinaryReader reader)
        {
            long firstHeaderPart = reader.ReadInt64();
            long secondHeaderPart = reader.ReadInt64();
            if (firstHeaderPart != HEADER_PART_1 || secondHeaderPart != HEADER_PART_2)
            {
                return false;
            }

            ResourceInfos = new BundleResourceInfo[reader.ReadInt32()];

            using (MemoryStream ms = new MemoryStream(reader.ReadBytes(reader.ReadInt32())))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < ResourceInfos.Length; i++)
                    {
                        var res = ResourceInfos[i];
                        res.Read(br);
                        string resourceName = res.NameId;
                        if (!string.IsNullOrEmpty(resourceName))
                        {
                            ResourcesNamesMap[resourceName] = i;
                        }
                        ResourceInfos[i] = res;
                    }
                }
            }

            return true;
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(HEADER_PART_1);
            writer.Write(HEADER_PART_2);

            byte[] binaryResourceInfos = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < ResourceInfos.Length; i++)
                    {
                        ResourceInfos[i].Write(bw);
                    }
                }

                binaryResourceInfos = ms.ToArray();
            }
            writer.Write(ResourceInfos.Length);
            writer.Write(binaryResourceInfos.Length);
            writer.Write(binaryResourceInfos);

        }
    }
}