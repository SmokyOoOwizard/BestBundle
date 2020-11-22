namespace BestBundle
{
    public interface IResource
    {
        string ResourceType { get; }

        bool Restore(in byte[] rawData);
        bool Save(out byte[] rawData);
    }
}