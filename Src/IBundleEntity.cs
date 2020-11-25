namespace BestBundle
{
    public interface IBundleEntity
    {
        string EntityType { get; }

        bool Restore(in byte[] rawData);
        bool Save(out byte[] rawData);
    }
}