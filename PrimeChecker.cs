/// Name: Aahish Balimane
/// File: PrimeChecker.cs
using System;
using System.Numerics;

namespace Messenger{
    
    /// <summary>
    /// Class to check is a number is prime or not
    /// </summary>
    static class CheckPrime{
        
        /// <summary>
        /// This is a provided function. This can be called on a BigInteger
        /// to check if it is a prime number of not
        /// </summary>
        /// <param name="value"> the number to be checked for is it is a prime number</param>
        /// <param name="witnesses"></param>
        /// <returns>True if the number is prime, else false</returns>
        public static Boolean IsProbablyPrime(this BigInteger value, int witnesses = 10) {
            if (value <= 1) return false;
            if (witnesses <= 0) witnesses = 10;
            BigInteger d = value - 1;
            var s = 0;
            while (d % 2 == 0) {
                d /= 2;
                s += 1;
            }
            Byte[] bytes = new Byte[value.ToByteArray().LongLength];
            BigInteger a;
            for (int i = 0; i < witnesses; i++) {
                do {
                    var Gen = new Random();
                    Gen.NextBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= value - 2);
                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1) continue;
                for (int r = 1; r < s; r++) {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == 1) return false;
                    if (x == value - 1) break;
                }
                if (x != value - 1) return false;
            }
            return true;
        }
    }
}