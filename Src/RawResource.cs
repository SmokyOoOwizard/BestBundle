namespace BestBundle
{
    internal struct RawResource
    {
        // 0x0F - Header
        // 0x04 - Local Id
        // 0x04 - Length
        // ?    - Resource

        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;
        public int LocalId;
        public int ResourceLength;
        public byte[] Resource;
    }
}