using System.Text;

namespace CryptoChain.Core.Abstractions
{
    public interface ICryptAlgorithm
    {
        string Encrypt(string text);
        string Encrypt(string text, Encoding encoder);
        byte[] Encrypt(byte[] data);
        string Decrypt(string text);
        string Decrypt(string text, Encoding encoder);
        byte[] Decrypt(byte[] data);
    }
}