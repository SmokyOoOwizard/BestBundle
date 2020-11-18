namespace BestBundle
{
    internal struct BundleChunkInfo
    {
        // 0x04 - Local Id
        // 0x04 - Resources Count
        // 0x04 - Chunk Length
        // 0x04 - Chunk Position

        public int LocalId;
        public int ResourcesCount;
        public int ChunkLength;
        public int ChunkPosition;
    }
}