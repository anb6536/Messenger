/// Name: Aahish Balimane
/// File: Generator.cs

using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Numerics;

namespace Messenger
{
    /// <summary>
    /// The class used to generate random numbers and check
    /// if they are prime
    /// </summary>
    class Generator{
        private static RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        private static object outputLock = new Object();
        
        /// <summary>
        /// This function is used to generate numbers and see if they are prime.
        /// If the numbers are prime, they are printed to the console.
        /// </summary>
        /// <param name="size"> Bit size for the number </param>
        /// <param name="numTimes"> The number of prime numbers to be generated and printed </param>
        public BigInteger generatePrime(int size, int numTimes){
            int count = 0;
            BigInteger finalNum = new BigInteger();
            var num = Parallel.For(0, Int32.MaxValue, (index, state) => {
                if(count == numTimes){
                    state.Stop();
                }
                Byte[] arr = new Byte[size/8];
                random.GetBytes(arr);
                BigInteger number = new BigInteger(arr);
                if(number.IsProbablyPrime()){
                    lock(outputLock){
                        if(!state.IsStopped){
                            count++;
                            finalNum = number;
                        }
                    }
                }
            });
            return finalNum;
        }
    }
}