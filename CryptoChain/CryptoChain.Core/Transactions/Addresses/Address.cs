using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography;

namespace CryptoChain.Core.Transactions.Addresses
{
    public class Address : ISerializable
    {
        public int Length => 1 + 4 + Data.Length;

        public AddressType Prefix { get; set; }
        public byte[] Data { get; set; }
        public Checksum Checksum { get; set; }

        public string Value => ToString();

        public bool IsValid => Checksum.Validate(GetChecksumData());

        /// <summary>
        /// Create a new address
        /// </summary>
        /// <param name="data">The data it must contain</param>
        /// <param name="addressType">The type</param>
        public Address(byte[] data, AddressType addressType)
        {
            Data = data;
            Prefix = addressType;
            Checksum = new Checksum(GetChecksumData(), 4);
        }

        public Address(string address) : this(Base58.Decode(address)){}
        public Address(byte[] serialized)
        {
            Prefix = (AddressType)serialized[0];
            Data = serialized[1..^4];
            Checksum = new Checksum(serialized[^4..]);
        }
        

        private byte[] GetChecksumData()
        {
            byte[] buffer = new byte[1 + Data.Length];
            buffer[0] = (byte)Prefix;
            Data.CopyTo(buffer, 1);
            return buffer;
        }

        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            buffer[0] = (byte) Prefix;
            Data.CopyTo(buffer, 1);
            Checksum.Serialize().CopyTo(buffer, buffer.Length - 4);
            return buffer;
        }

        public override string ToString()
            => Base58.Encode(Serialize());
    }
}