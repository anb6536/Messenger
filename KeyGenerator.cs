/// Name: Aahish Balimane
/// File: KeyGenerator.cs
using System;
using System.Buffers.Text;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace Messenger
{
    /// <summary>
    /// The KeyGenerator class generated the public and the private key and
    /// stores them in a file to be used to encode and decode the messages
    /// that will be sent or received from the server 
    /// </summary>
    /// <author>Aahish Balimane</author>
    public class KeyGenerator
    {
        public int len { get; set; }
        public Byte[] publicKey { get; set; }
        public Byte[] privateKey { get; set; }
        
        /// <summary>
        /// The constructor for the KeyGenerator class
        /// </summary>
        /// <param name="len">The length of the key to be generated</param>
        public KeyGenerator(int len)
        {
            this.len = len;
            generateKey();
            filePrivateKey();
            filePublicKey();
            Console.WriteLine();
        }

        /// <summary>
        /// This function gererates the public and the private keys
        /// </summary>
        public void generateKey()
        {
            Generator generator = new Generator();
            Random rand = new Random(1);
            int randomNum = rand.Next(1, this.len);
            var pLen = (this.len / 2) - randomNum;
            var qLen = (this.len / 2) + randomNum;
            
            var p = generator.generatePrime(pLen, 1);
            var q = generator.generatePrime(qLen, 1);

            var N = p * q;
            var r = (p - 1) * (q - 1);
            var E = new BigInteger(65537);
            var D = modInverse(E, r);
            
            //N which is common for both keys
            var nArr = N.ToByteArray();
            var nLen = nArr.Length;
            var smallN = BitConverter.GetBytes(nLen);
            Array.Reverse(smallN);

            //PUBLIC KEY
            // format: eeeeEEEEEEEEEEEE.....EEnnnnNNNNNNN.....NNN
            var eArr = E.ToByteArray();
            var eLen = eArr.Length;
            var smallE = BitConverter.GetBytes(eLen);
            Array.Reverse(smallE);

            this.publicKey = new Byte[4 + eArr.Length + 4 + nArr.Length];
            
            Array.Copy(smallE, 0, this.publicKey, 0, smallE.Length);
            
            Array.Copy(eArr, 0, this.publicKey, smallE.Length, 
                eArr.Length);
            
            Array.Copy(smallN, 0, this.publicKey, 
                smallE.Length + eArr.Length, smallN.Length);
            
            Array.Copy(nArr, 0, this.publicKey, smallE.Length + 
                                                eArr.Length + smallN.Length, nArr.Length);

            //PRIVATE KEY
            // format: ddddDDDDDDDD.....DDnnnnNNNNNNNNN......NNNNN
            var dArr = D.ToByteArray();
            var dLen = dArr.Length;
            var smallD = BitConverter.GetBytes(dLen);
            Array.Reverse(smallD);

            this.privateKey = new Byte[4 + dArr.Length + 4 + nArr.Length];
            Array.Copy(smallD, 0, this.privateKey, 0, smallD.Length);
            
            Array.Copy(dArr, 0, this.privateKey, smallD.Length, 
                dArr.Length);
            
            Array.Copy(smallN, 0, this.privateKey, smallD.Length + 
                                                   dArr.Length, smallN.Length);
            
            Array.Copy(nArr, 0, this.privateKey, smallD.Length + 
                                                 dArr.Length + smallN.Length, nArr.Length);
        }

        /// <summary>
        /// This is a provided function that calculates the mod inverse of a number
        /// with respect to the other
        /// </summary>
        /// <param name="a">The number to find the mod inverse of</param>
        /// <param name="n">The number with respect to the mod inverse is found</param>
        /// <returns>The answer to mod inverse</returns>
        public BigInteger modInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a>0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t*x;
                v = x;
            }

            v %= n;
            if (v < 0)
            {
                v = (v + n) % n;
            }

            return v;
        }


        /// <summary>
        /// This function is used to write the email and the private key that is generated into a file to be
        /// stored locally.
        /// </summary>
        public void filePrivateKey()
        {
            try
            {
                StreamWriter sw = new StreamWriter("private.key");
                sw.WriteLine("email:");
                sw.Write("key: ");
                sw.WriteLine(Convert.ToBase64String(this.privateKey));
                sw.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Error with Private Key " + e);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// This function is used to write the email and the public keys that are generated in to a file to be
        /// stored locally
        /// </summary>
        public void filePublicKey()
        {
            try
            {
                StreamWriter sw = new StreamWriter("public.key");
                sw.WriteLine("email: anb6536@rit.edu");
                sw.Write("key: ");
                sw.WriteLine(Convert.ToBase64String(this.publicKey));
                sw.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Error with Public Key " + e);
                Environment.Exit(1);
            }
        }
    }
}