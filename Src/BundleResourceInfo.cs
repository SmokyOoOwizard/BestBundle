namespace BestBundle
{
    internal struct BundleResourceInfo
    {
        // 0x04 - Local Id
        // 0x04 - Name Id Length
        // ?    - Name Id
        // 0x04 - Resource Type Length
        // ?    - Resource Type
        // 0x04 - Resource Length
        // 0x04 - Resource Position

        public int LocalId;
        public string NameId;
        public string ResourceType;
        public int ResourceLength;
        public int ResourcePosition;
    }
}