/// Name: Aahish Balimane
/// File: Msg.cs

using System;
using System.Buffers.Text;

namespace Messenger
{
    /// <summary>
    /// This is the class used to store the key objects sent and received from the server.
    /// </summary>
    public class Keys
    {
        public string email { get; set; }
        public string key { get; set; }
    }
    

    /// <summary>
    /// This class is used to store the message and the email sent and received from the server
    /// </summary>
    public class Msgs
    {
        public string email { get; set; }
        public string content { get; set; }
    }
}