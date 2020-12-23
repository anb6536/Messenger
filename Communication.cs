/// Name: Aahish Balimane
/// File: Communication.cs

using System;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Messenger
{

    /// <summary>
    /// This class is used to send and receive keys and messages to and from the server.
    /// </summary>
    public class Communication
    {
        private const string Host = "http://kayrun.cs.rit.edu";
        private const string Port = ":5000";
        private const string Message = "/Message";
        private const string Key = "/Key";
        private readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// The function sends the public key to the server to enable other users
        /// to encode the message to be sent.
        /// </summary>
        /// <param name="email">The users email to which the public key belongs</param>
        /// <returns>Task object representing the async operation</returns>
        public async Task SendKey(string email)
        {
            try
            {
                string line, priEmail, publicKey, privateKey;

                if (!File.Exists("public.key") || !File.Exists("private.key"))
                {
                    Console.WriteLine("Keys haven't been generated! Use keyGen first!\n");
                    Environment.Exit(1);
                }

                StreamReader sr1 = new StreamReader("public.key");
                line = sr1.ReadLine();
                string[] emailLine1 = line.Split(" ");
                line = sr1.ReadLine();
                string[] keyLine1 = line.Split(" ");
                publicKey = keyLine1[1];
                sr1.Close();

                StreamReader sr2 = new StreamReader("private.key");
                line = sr2.ReadLine();
                line = line + " " + email;
                priEmail = line;
                line = sr2.ReadLine();
                string[] keyLine2 = line.Split(" ");
                privateKey = keyLine2[1];
                sr2.Close();

                StreamWriter sw = new StreamWriter("private.key");
                sw.WriteLine(priEmail);
                sw.WriteLine("key: " + privateKey);
                sw.Close();

                var cont = new Keys();
                cont.email = email;
                cont.key = publicKey;

                string json = JsonConvert.SerializeObject(cont, Formatting.Indented);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage =
                    await _client.PutAsync("http://kayrun.cs.rit.edu:5000/Key/"+email, httpContent);
                responseMessage.EnsureSuccessStatusCode();
                
                Console.WriteLine("Key saved\n");
                Environment.Exit(0);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error with Key");
                Environment.Exit(1);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error with Key");
                Environment.Exit(1);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Error connecting to the server");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// The function is used to get the Key of a particular user to her their public key, which is used
        /// to encode the messages sent to them. The key is saved in a file with their email as the name of
        /// the file with the .key suffix
        /// </summary>
        /// <param name="email">The email of the user whose public key is to be fetched</param>
        /// <returns>Task object representing the async operation</returns>
        public async Task GetKey(string email)
        {
            try
            {
                string msg = await _client.GetStringAsync(Host + Port + Key + "/" + email);
                var keyObj = JsonConvert.DeserializeObject<Keys>(msg);
                StreamWriter sw = new StreamWriter(email + ".key");
                sw.WriteLine("email: " + keyObj.email);
                sw.Write("key: ");
                sw.WriteLine(keyObj.key);
                sw.Close();
                Console.WriteLine();
                Environment.Exit(0);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error with Key");
                Environment.Exit(1);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error with Key");
                Environment.Exit(1);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Error connecting to the server");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// This function is used to fetch messages sent by other users to us.
        /// </summary>
        /// <param name="email">The users email ID to fetch the messages sent to him</param>
        /// <returns>A task object representing the async operation</returns>
        public async Task GetMsg(string email)
        {
            try
            {
                StreamReader sr2 = new StreamReader("private.key");
                var line = sr2.ReadLine();
                var key = sr2.ReadLine().Split(" ")[1];
                string[] emails = line.Split(" ");
                var flag = false;
                for (int i = 0; i < emails.Length; i++)
                {
                    if (emails[i].Equals(email))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    Console.WriteLine("Fatal Error: Key for this email does not exist!");
                    Environment.Exit(1);
                }
                string json = await _client.GetStringAsync(Host + Port + Message + "/" + email);
                var message = JsonConvert.DeserializeObject<Msgs>(json);
                var content = message.content;
                var byteArr = Convert.FromBase64String(content);
                var bigInt = new BigInteger(byteArr);
                var keyByteArr = Convert.FromBase64String(key);
                var dArr = keyByteArr[0..4];
                Array.Reverse(dArr);
                var d = new BigInteger(dArr);
                var DEnd = (int)(4 + d);
                var DArr = keyByteArr[4 .. DEnd];
                var D = new BigInteger(DArr);
                var nEnd = DEnd + 4;
                var nArr = keyByteArr[DEnd .. nEnd];
                Array.Reverse(nArr);
                var n = new BigInteger(nArr);
                var NArr = keyByteArr[nEnd .. keyByteArr.Length];
                var N = new BigInteger(NArr);
                
                var P = BigInteger.ModPow(bigInt, D, N);
                var contArr = P.ToByteArray();
                var textMsg = Encoding.UTF8.GetString(contArr);
                Console.WriteLine(textMsg);
                Console.WriteLine();
                
                sr2.Close();
                Environment.Exit(0);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error with Key");
                Environment.Exit(1);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error with Key");
                Environment.Exit(1);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Error connecting to the server");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// This function is used to send a message to any other user. The message is encoded using their
        /// public key.
        /// </summary>
        /// <param name="email">The email of the user to who the message is to be sent</param>
        /// <param name="plaintext">The message to be sent to the user</param>
        /// <returns>The task object representing the async task</returns>
        public async Task SendMsg(string email, string plaintext)
        {
            try
            {
                StreamReader sr = new StreamReader(email+".key");
                var line = sr.ReadLine();
                var key = sr.ReadLine().Split(" ")[1];
                var byteArr = Encoding.ASCII.GetBytes(plaintext);
                var bigInt = new BigInteger(byteArr);
                var keyByteArr = Convert.FromBase64String(key);
                
                var eArr = keyByteArr[0..4];
                Array.Reverse(eArr);
                var e = new BigInteger(eArr);
                var EEnd = (int)(4 + e);
                var EArr = keyByteArr[4 .. EEnd];
                var E = new BigInteger(EArr);
                var nEnd = EEnd + 4;
                var nArr = keyByteArr[EEnd .. nEnd];
                Array.Reverse(nArr);
                var n = new BigInteger(nArr);
                var NArr = keyByteArr[nEnd .. keyByteArr.Length];
                var N = new BigInteger(NArr);
                
                var C = BigInteger.ModPow(bigInt, E, N);

                
                var cByte = C.ToByteArray();
                var encoded = Convert.ToBase64String(cByte);
                var msg = new Msgs();
                msg.email = email;
                msg.content = encoded;
                
                string json = JsonConvert.SerializeObject(msg, Formatting.Indented);
                var httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage =
                    await _client.PutAsync(Host + Port + Message + "/" + email, httpContent);
                responseMessage.EnsureSuccessStatusCode();
                
                Console.WriteLine("Message Written\n");
                Environment.Exit(0);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Key does not exist for " + email);
                Environment.Exit(1);
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error with Key");
                Environment.Exit(1);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Error connecting to the server");
                Environment.Exit(1);
            }
        }
    }
}