/// Name: Aahish Balimane
/// File: Program.cs 

using System;
using System.Net.Http;

namespace Messenger
{
    /// <summary>
    /// This is the main function that calls the other classes and the function used to
    /// perform the required for the project
    /// </summary>
    class Program
    {
        /// <summary>
        /// This function is used to display the error message when the user inputs the wrong command on the
        /// command line
        /// </summary>
        public void usage()
        {
            Console.WriteLine("Dotnet run <option> <other arguments>");
            Console.WriteLine("Options:");
            Console.WriteLine("\tkeyGen <argument: keysize>");
            Console.WriteLine("\tsendKey <argument: email>");
            Console.WriteLine("\tgetKey <argument: email>");
            Console.WriteLine("\tsendMsg <arguments: email, plaintext>");
            Console.WriteLine("\tgetMsg <argument: email>");
            Environment.Exit(0);
        }
        
        /// <summary>
        /// This is the main function that redirects the flow to the specified functions to perform particular tasks
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            var prog = new Program();
            var comm = new Communication();
            if (args.Length == 0)
            {
                prog.usage();
            }
            var option = args[0];
            if (option.Equals("keyGen"))
            {
                if (args.Length != 2)
                {
                    prog.usage();
                }
                else
                {
                    var keysize = int.Parse(args[1]);
                    var generator = new KeyGenerator(keysize);
                }
            }
            else if (option.Equals("sendKey"))
            {
                if (args.Length != 2)
                {
                    prog.usage();
                }
                else
                {
                    var email = args[1];
                    comm.SendKey(email).Wait();
                    //Console.Read();
                }
            }
            
            else if (option.Equals("getKey"))
            {
                if (args.Length != 2)
                {
                    prog.usage();
                }
                else
                {
                    var email = args[1];
                    comm.GetKey(email).Wait();
                    //Console.Read();
                }
            }
            
            else if (option.Equals("sendMsg"))
            {
                if (args.Length != 3)
                {
                    prog.usage();
                }
                else
                {
                    var email = args[1];
                    var plaintext = args[2];
                    comm.SendMsg(email, plaintext).Wait();
                    //Console.Read();
                }
            }
            
            else if (option.Equals("getMsg"))
            {
                if (args.Length != 2)
                {
                    prog.usage();
                }
                else
                {
                    var email = args[1];
                    comm.GetMsg(email).Wait();
                    //Console.Read();
                }
            }

            else
            {
                prog.usage();
            }
        }
    }
}
