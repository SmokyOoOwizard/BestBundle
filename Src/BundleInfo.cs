namespace BestBundle
{
    internal struct BundleInfo
    {
        // 0x0F - Header
        // 0x04 - Name Length
        // ?    - Name
        // 0x04 - Bundle Create Time

        public const long HEADER_PART_1 = 0xFF;
        public const long HEADER_PART_2 = 0xFF;

        public string Name;
        public long CreateTime;
    }
}