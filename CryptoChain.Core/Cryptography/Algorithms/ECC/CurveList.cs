using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CryptoChain.Core.Cryptography.Algorithms.ECC.Curves;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public static class CurveCollection
    {
        public static Curve GetById(byte id)
            => Curves.First(x => x.Id == id);

        public static Curve GetByName(string name)
            => Curves.First(x => x.Name?.ToLower() == name.ToLower());
        
        public static List<Curve> Curves = new()
        {
            /*new ShortWeierstrassCurve
            {
                Name = "brainpoolP160r1",
                A = Mathematics.FromHex("0x340e7be2a280eb74e2be61bada745d97e8f7c300"),
                B = Mathematics.FromHex("0x1e589a8595423412134faa2dbdec95c8d8675e58"),
                P = Mathematics.FromHex("0xe95e4a5f737059dc60dfc7ad95b3d8139515620f"),
                N = Mathematics.FromHex("0xe95e4a5f737059dc60df5991d45029409e60fc09"),
                H = 1,
                G = new Point(Mathematics.FromHex("0xbed5af16ea3f6a4f62938c4631eb5af7bdbcdbc3"),
                    Mathematics.FromHex("0x1667cb477a1a8ec338f94741669c976316da6321"))
            }*/

            new ShortWeierstrassCurve
            {
                Name = "secp256k1",
                Id = 0,
                A = 0,
                B = 7,
                P = Mathematics.FromHex("0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f"),
                N = Mathematics.FromHex("0xfffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141"),
                H = 1,
                G = new Point(Mathematics.FromHex("0x79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798"),
                    Mathematics.FromHex("0x483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8"))
            },
            new ShortWeierstrassCurve
            {
                Name = "secp256r1",
                Id = 1,
                A = Mathematics.FromHex("0xffffffff00000001000000000000000000000000fffffffffffffffffffffffc"),
                B = Mathematics.FromHex("0x5ac635d8aa3a93e7b3ebbd55769886bc651d06b0cc53b0f63bce3c3e27d2604b"),
                P = Mathematics.FromHex("0xffffffff00000001000000000000000000000000ffffffffffffffffffffffff"),
                N = Mathematics.FromHex("0xffffffff00000000ffffffffffffffffbce6faada7179e84f3b9cac2fc632551"),
                H = 1,
                G = new Point(Mathematics.FromHex("0x6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296"),
                    Mathematics.FromHex("0x4fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5"))
            },
            
            //...
            new TwistedEdwardsCurve
            {
                Name = "Ed25519",
                Id = 100,
                A = -1,
                D = BigInteger.Parse("37095705934669439343138083508754565189542113879843219016388785533085940283555"),
                P = BigInteger.Pow(2, 255) - 19,
                N = BigInteger.Pow(2, 252) + BigInteger.Parse("27742317777372353535851937790883648493"),
                H = 8,
                G = new Point(Mathematics.FromHex("0x216936d3cd6e53fec0a4e231fdd6dc5c692cc7609525a7b2c9562d608f25d51a"),
                    Mathematics.FromHex("0x6666666666666666666666666666666666666666666666666666666666666658")),
                Flag = CurveFlags.NEED_SET_MSB | CurveFlags.NEED_SET_PSG
            },
            new TwistedEdwardsCurve()
            {
                Name = "Ed448",
                Id = 101,
                A = 1,
                D = -39081,
                P = Mathematics.FromHex("0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffeffffffffffffffffffffffffffffffffffffffffffffffffffffffff"),
                N = Mathematics.FromHex("0x3fffffffffffffffffffffffffffffffffffffffffffffffffffffff7cca23e9c44edb49aed63690216cc2728dc58f552378c292ab5844f3"),
                H = 4,
                G = new Point(Mathematics.FromHex("0x4F1970C66BED0DED221D15A622BF36DA9E146570470F1767EA6DE324A3D3A46412AE1AF72AB66511433B80E18B00938E2626A82BC70CC05E"),
                    Mathematics.FromHex("0x693F46716EB6BC248876203756C9C7624BEA73736CA3984087789C1E05A0C2D73AD3FF1CE67C39C4FDBD132C4ED7C8AD9808795BF230FA14"))
            }
        };
    }
}