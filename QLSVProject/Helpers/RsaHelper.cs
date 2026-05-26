using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace QLSVProject.Helpers
{
    internal class RsaHelper
    {
        public static RSA CreateRSA(string str)
        {
            byte[] seed = CreateSeed(str);
            DeterministicRandom rng = new DeterministicRandom(seed);

            BigInteger e = 65537;
            BigInteger p, q, phi;

            while (true)
            {
                p = GeneratePrime(1024, rng);
                q = GeneratePrime(1024, rng);

                if (p == q) continue;

                phi = (p - 1) * (q - 1);

                if (BigInteger.GreatestCommonDivisor(e, phi) == 1)
                    break;
            }

            BigInteger n = p * q;
            BigInteger d = ModInverse(e, phi);

            RSAParameters rsaParams = BuildParameters(p, q, d, e, n);

            // Dùng RSACng thay vì RSA.Create() để hỗ trợ OaepSHA256 trên Windows
            RSA rsa = new RSACng();
            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        public static byte[] Encrypt(RSA rsa, byte[] plainText)
        {
            return rsa.Encrypt(plainText, RSAEncryptionPadding.OaepSHA256);
        }

        public static byte[] Decrypt(RSA rsa, byte[] cipher)
        {
            return rsa.Decrypt(cipher, RSAEncryptionPadding.OaepSHA256);
        }

        private static byte[] CreateSeed(string str)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(str));
            }
        }

        // Danh sách số nguyên tố nhỏ để lọc sơ bộ
        private static readonly int[] SmallPrimes = new int[]
        {
            3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47,
            53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107,
            109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167,
            173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
            233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283,
            293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359,
            367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431,
            433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491,
            499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571,
            577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641,
            643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709,
            719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787,
            797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859,
            863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
            947, 953, 967, 971, 977, 983, 991, 997
        };

        private static BigInteger GeneratePrime(int bits, DeterministicRandom rng)
        {
            while (true)
            {
                byte[] bytes = rng.NextBytes(bits / 8);

                bytes[0] |= 0x01;                  // force odd
                bytes[bytes.Length - 1] &= 0x7F;   // force positive
                bytes[bytes.Length - 1] |= 0x40;   // force high bit

                BigInteger p = new BigInteger(bytes);
                if (p <= BigInteger.One)
                    continue;

                // try divide small primes
                bool divisible = false;
                foreach (int sp in SmallPrimes)
                {
                    if (p == sp)
                        break; 

                    if (p % sp == 0)
                    {
                        divisible = true;
                        break;
                    }
                }

                if (divisible)
                    continue;

                // call miller rabin check
                if (IsProbablePrime(p, 5))
                    return p;
            }
        }

        // miller rabin check prime
        private static bool IsProbablePrime(BigInteger n, int k = 10)
        {
            // base check
            if (n < 2) return false;
            if (n == 2 || n == 3) return true;
            if (n % 2 == 0) return false;

            BigInteger d = n - 1;
            int s = 0;

            while (d % 2 == 0) { d /= 2; s++; }

            byte[] nBytes = n.ToByteArray();
            byte[] bytes = new byte[nBytes.Length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < k; i++)
                {
                    BigInteger a;
                    do
                    {
                        rng.GetBytes(bytes);

                        byte[] unsigned = new byte[bytes.Length + 1];
                        Buffer.BlockCopy(bytes, 0, unsigned, 0, bytes.Length);
                        unsigned[unsigned.Length - 1] = 0x00;

                        a = new BigInteger(unsigned);
                    }
                    while (a < 2 || a >= n - 2);

                    BigInteger x = BigInteger.ModPow(a, d, n);

                    if (x == 1 || x == n - 1) continue;

                    bool composite = true;

                    for (int r = 1; r < s; r++)
                    {
                        x = BigInteger.ModPow(x, 2, n);
                        if (x == n - 1) { composite = false; break; }
                    }

                    if (composite) return false;
                }
            }

            return true;
        }

        private static BigInteger ModInverse(BigInteger e, BigInteger phi)
        {
            BigInteger t = 0, newT = 1;
            BigInteger r = phi, newR = e;

            while (newR != 0)
            {
                BigInteger q = r / newR;

                BigInteger tempT = newT;
                newT = t - q * newT;
                t = tempT;

                BigInteger tempR = newR;
                newR = r - q * newR;
                r = tempR;
            }

            if (t < 0) t += phi;
            return t;
        }

        private static RSAParameters BuildParameters(BigInteger p, BigInteger q, BigInteger d, BigInteger e, BigInteger n)
        {
            BigInteger dp = d % (p - 1);
            BigInteger dq = d % (q - 1);
            BigInteger inverseQ = ModInverse(q, p);

            int modulusLen = ToBigEndianBytes(n).Length;
            int halfLen = (modulusLen + 1) / 2;

            return new RSAParameters
            {
                Modulus = ToBigEndianBytes(n, modulusLen),
                Exponent = ToBigEndianBytes(e),
                D = ToBigEndianBytes(d, modulusLen),
                P = ToBigEndianBytes(p, halfLen),
                Q = ToBigEndianBytes(q, halfLen),
                DP = ToBigEndianBytes(dp, halfLen),
                DQ = ToBigEndianBytes(dq, halfLen),
                InverseQ = ToBigEndianBytes(inverseQ, halfLen),
            };

            
        }

        private static byte[] ToBigEndianBytes(BigInteger value, int targetLength = 0)
        {
            byte[] le = value.ToByteArray();
            Array.Reverse(le);

            int start = 0;
            while (start < le.Length - 1 && le[start] == 0x00)
                start++;

            byte[] be = new byte[le.Length - start];
            Buffer.BlockCopy(le, start, be, 0, be.Length);

            if (targetLength > 0 && be.Length < targetLength)
            {
                byte[] padded = new byte[targetLength];
                Buffer.BlockCopy(be, 0, padded, targetLength - be.Length, be.Length);
                return padded;
            }

            return be;
        }
    }

    internal class DeterministicRandom
    {
        private readonly byte[] seed;
        private ulong counter = 0;

        public DeterministicRandom(byte[] seed)
        {
            this.seed = seed;
        }

        public byte[] NextBytes(int length)
        {
            byte[] result = new byte[length];
            int offset = 0;

            while (offset < length)
            {
                byte[] counterBytes = BitConverter.GetBytes(counter++);

                using (HMACSHA256 hmac = new HMACSHA256(seed))
                {
                    byte[] hash = hmac.ComputeHash(counterBytes);
                    int copy = Math.Min(hash.Length, length - offset);
                    Buffer.BlockCopy(hash, 0, result, offset, copy);
                    offset += copy;
                }
            }

            return result;
        }
    }
}