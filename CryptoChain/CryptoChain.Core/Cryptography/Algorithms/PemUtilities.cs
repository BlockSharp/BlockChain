using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using CryptoChain.Core.Cryptography.Algorithms.ECC;

namespace CryptoChain.Core.Cryptography.Algorithms
{
    public static class PemUtilities
    {
        /// <summary>
        /// Export private (including public) key from MS RSACryptoServiceProvider into OpenSSH PEM string
        /// slightly modified from https://stackoverflow.com/a/23739932/2860309
        /// </summary>
        /// <param name="csp">The csp containing the key</param>
        /// <returns>PEM string</returns>
        public static string ExportPrivateKey(RSACryptoServiceProvider csp, bool armor = true, bool base64Encode = true)
        {
            if (csp is null)
                throw new ArgumentNullException(paramName: nameof(csp));

            if (csp.PublicOnly)
                throw new ArgumentException(
                    message: "CSP does not contain a private key",
                    paramName: nameof(csp));

            string result; // filled at end of using
            using (StringWriter outputStream = new StringWriter())
            {
                var parameters = csp.ExportParameters(true);
                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write((byte)0x30); // SEQUENCE
                    using (var innerStream = new MemoryStream())
                    using (var innerWriter = new BinaryWriter(innerStream))
                    {
                        EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                        EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                        EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                        EncodeIntegerBigEndian(innerWriter, parameters.D);
                        EncodeIntegerBigEndian(innerWriter, parameters.P);
                        EncodeIntegerBigEndian(innerWriter, parameters.Q);
                        EncodeIntegerBigEndian(innerWriter, parameters.DP);
                        EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                        EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                        var length = (int)innerStream.Length;
                        EncodeLength(writer, length);
                        writer.Write(innerStream.GetBuffer(), 0, length);
                    }

                    // WriteLine terminates with \r\n, we want only \n
                    if (armor) outputStream.Write("-----BEGIN RSA PRIVATE KEY-----\n");
                    // Output as Base64 with lines chopped at 64 characters
                    if (base64Encode)
                    {
                        var base64 = Convert.ToBase64String(
                            inArray: stream.GetBuffer(),
                            offset: 0,
                            length: (int)stream.Length).ToCharArray();
                        for (var i = 0; i < base64.Length; i += 64)
                        {
                            outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                            outputStream.Write("\n");
                        }
                    }
                    else
                    {
                        outputStream.Write(stream.GetBuffer());
                    }
                    if (armor) outputStream.Write("-----END RSA PRIVATE KEY-----");
                }

                result = outputStream.ToString();
            }
            return result;
        }

        
        /// <summary>
        /// Export public key from MS RSACryptoServiceProvider into OpenSSH PEM string
        /// slightly modified from https://stackoverflow.com/a/28407693
        /// </summary>
        /// <param name="csp"></param>
        /// <returns></returns>
        public static string ExportPublicKey(RSACryptoServiceProvider csp, bool armor = true, bool base64Encode = true)
        {
            if (csp is null)
                throw new ArgumentNullException(paramName: nameof(csp));

            string result; // filled at end
            using (StringWriter outputStream = new StringWriter())
            {
                var parameters = csp.ExportParameters(false);
                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write((byte)0x30); // SEQUENCE
                    using (var innerStream = new MemoryStream())
                    using (var innerWriter = new BinaryWriter(innerStream))
                    {
                        innerWriter.Write((byte)0x30); // SEQUENCE
                        EncodeLength(innerWriter, 13);
                        innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                        var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                        EncodeLength(innerWriter, rsaEncryptionOid.Length);
                        innerWriter.Write(rsaEncryptionOid);
                        innerWriter.Write((byte)0x05); // NULL
                        EncodeLength(innerWriter, 0);
                        innerWriter.Write((byte)0x03); // BIT STRING
                        using (var bitStringStream = new MemoryStream())
                        using (var bitStringWriter = new BinaryWriter(bitStringStream))
                        {
                            bitStringWriter.Write((byte)0x00); // # of unused bits
                            bitStringWriter.Write((byte)0x30); // SEQUENCE
                            using (var paramsStream = new MemoryStream())
                            using (var paramsWriter = new BinaryWriter(paramsStream))
                            {
                                EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                                EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                                var paramsLength = (int)paramsStream.Length;
                                EncodeLength(bitStringWriter, paramsLength);
                                bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                            }
                            var bitStringLength = (int)bitStringStream.Length;
                            EncodeLength(innerWriter, bitStringLength);
                            innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                        }
                        var length = (int)innerStream.Length;
                        EncodeLength(writer, length);
                        writer.Write(innerStream.GetBuffer(), 0, length);
                    }

                    // WriteLine terminates with \r\n, we want only \n
                    if (armor) outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
                    if (base64Encode)
                    {
                        var base64 = Convert.ToBase64String(
                            inArray: stream.GetBuffer(),
                            offset: 0,
                            length: (int)stream.Length).ToCharArray();

                        for (var i = 0; i < base64.Length; i += 64)
                        {
                            outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                            outputStream.Write("\n");
                        }
                    }
                    else
                    {
                        outputStream.Write(stream.GetBuffer());
                    }
                    if (armor) outputStream.Write("-----END PUBLIC KEY-----");
                }
                result = outputStream.ToString();
            }

            return result;
        }

        /// <summary>
        /// https://stackoverflow.com/a/23739932/2860309
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/23739932/2860309
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <param name="forceUnsigned"></param>
        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[]? value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            Debug.Assert(value != null, nameof(value) + " != null");
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }
    }
}