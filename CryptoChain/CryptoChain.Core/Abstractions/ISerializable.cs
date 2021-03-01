namespace CryptoChain.Core.Abstractions
{
    public interface ISerializable
    {
        int Length { get; }
        byte[] Serialize();
    }
}