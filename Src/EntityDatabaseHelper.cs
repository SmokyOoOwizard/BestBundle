using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BestBundle
{
    internal static class EntityDatabaseHelper
    {
        public static void CreateAndWriteDatabase(Stream output, Dictionary<string, IBundleEntity> entities)
        {
            if (!output.CanSeek || !output.CanWrite)
            {
                return;
            }

            var databaseStartPosition = output.Position;

            var r = entities.OrderBy(t => t.Key).ToArray();

            var entityInfos = new BundleEntityInfo[r.Length];

            var tmpDB = new BundleEntitiesDatabase();

            using (BinaryWriter bw = new BinaryWriter(output, System.Text.Encoding.UTF8, true))
            {

                // fill part info
                for (int i = 0; i < r.Length; i++)
                {
                    var item = r[i];

                    var info = entityInfos[i];

                    info.NameId = item.Key;
                    info.LocalId = i;
                    info.EntityType = item.Value.EntityType;

                    entityInfos[i] = info;
                }

                tmpDB.EntityInfos = entityInfos;
                tmpDB.Write(bw);

                // write entities
                for (int i = 0; i < r.Length; i++)
                {
                    var startPos = output.Position;

                    var entity = r[i];
                    var rawEntity = new RawEntity();
                    rawEntity.LocalId = i;
                    rawEntity.EntityType = entity.Value.EntityType;

                    entity.Value.Save(out byte[] data);
                    rawEntity.Entity = data;
                    rawEntity.EntityLength = data.Length;

                    rawEntity.Write(bw);

                    var info = entityInfos[i];
                    info.EntityPosition = startPos;
                    info.EntityLength = data.Length;

                    entityInfos[i] = info;
                }

                output.Seek(databaseStartPosition, SeekOrigin.Begin);

                tmpDB.EntityInfos = entityInfos;
                tmpDB.Write(bw); //write full info

            }
        }
    }
}