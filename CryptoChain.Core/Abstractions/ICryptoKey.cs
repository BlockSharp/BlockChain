using CryptoChain.Core.Cryptography.Algorithms;

namespace CryptoChain.Core.Abstractions
{
    public interface ICryptoKey : ISerializable
    {
        bool IsPrivate { get; }
        byte[] PublicKey { get; }
        byte[] PrivateKey { get; }
        int KeySize { get; }
        string ToXmlString(bool withPrivate = true);
        string ToPemString(bool withPrivate = true);
        byte[] ToArray(bool withPrivate = true);
    }
}