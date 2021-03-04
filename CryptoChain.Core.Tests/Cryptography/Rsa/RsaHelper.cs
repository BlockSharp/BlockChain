using System;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.RSA;

namespace CryptoChain.Core.Tests.Cryptography.Rsa
{
    public static class RsaHelper
    {
        public static CryptoRsa GetSeededRsa()
        {
            var spp = new SeededPrimePair(Convert.FromBase64String("AAgAAIfpAAAfvAAAyvdY0e44Eegibo+rhPO0Ow=="));
            return new CryptoRsa(spp.ToRsaKey());
        }
    }
}