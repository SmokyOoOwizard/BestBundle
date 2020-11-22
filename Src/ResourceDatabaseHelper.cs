using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BestBundle
{
    internal static class ResourceDatabaseHelper
    {
        public static void CreateAndWriteDatabase(Stream output, Dictionary<string, IResource> resources)
        {
            if (!output.CanSeek || !output.CanWrite)
            {
                return;
            }

            var databaseStartPosition = output.Position;

            var r = resources.OrderBy(t => t.Key).ToArray();

            var resourceInfos = new BundleResourceInfo[r.Length];

            var tmpDB = new BundleResourceDatabase();

            using (BinaryWriter bw = new BinaryWriter(output, System.Text.Encoding.UTF8, true))
            {

                // fill part info
                for (int i = 0; i < r.Length; i++)
                {
                    var item = r[i];

                    var info = resourceInfos[i];

                    info.NameId = item.Key;
                    info.LocalId = i;
                    info.ResourceType = item.Value.ResourceType;

                    resourceInfos[i] = info;
                }

                tmpDB.ResourceInfos = resourceInfos;
                tmpDB.Write(bw);

                // write resource
                for (int i = 0; i < r.Length; i++)
                {
                    var startPos = output.Position;

                    var resource = r[i];
                    var rawResource = new RawResource();
                    rawResource.LocalId = i;
                    rawResource.ResourceType = resource.Value.ResourceType;

                    resource.Value.Save(out byte[] data);
                    rawResource.Resource = data;
                    rawResource.ResourceLength = data.Length;

                    rawResource.Write(bw);

                    var info = resourceInfos[i];
                    info.ResourcePosition = startPos;
                    info.ResourceLength = data.Length;

                    resourceInfos[i] = info;
                }

                output.Seek(databaseStartPosition, SeekOrigin.Begin);

                tmpDB.ResourceInfos = resourceInfos;
                tmpDB.Write(bw); //write full info

            }
        }
    }
}