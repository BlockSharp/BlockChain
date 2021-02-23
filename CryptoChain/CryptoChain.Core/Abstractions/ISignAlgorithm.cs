using System.Security.Cryptography;
using System.Text;

namespace CryptoChain.Core.Abstractions
{
    public interface ISignAlgorithm
    {
        bool IsPrivate { get; }
        ICryptoKey Key { get; }
        byte[] Sign(byte[] data);
        byte[] Sign(byte[] data, HashAlgorithm algorithm);
        bool Verify(byte[] data, byte[] signedData);
        bool Verify(byte[] data, HashAlgorithm algorithm, byte[] signedData);
    }
}