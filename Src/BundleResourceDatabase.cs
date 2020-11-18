namespace BestBundle
{
    internal class BundleResourceDatabase
    {
        // 0x0F - Header
        // 0x04 - Chunk Infos Count
        // 0x04 - Chunk Infos Length
        // ?    - Chunk Infos
        // 0x04 - Resource Infos Count
        // 0x04 - Resource Infos Length
        // ?    - Resource Infos


        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;

        private BundleChunkInfo[] ChunkInfos;
        private BundleResourceInfo[] ResourceInfos;
    }
}