
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace WebSockServer
{
    internal class ClientConnection
    {
        private static int nextId = 0;

        private TcpClient client = null!;
        private NetworkStream stream = null!;
        private Action<int, string> onReceive = null!;
        private Thread worker = null!;
        private int id = 0;
        private bool running = false;

        public bool Running { get {return running; } }
        
        public int Id {  get { return id;} }

        public ClientConnection(TcpClient client, Action<int, string> onReceive)
        {
            this.client = client;
            this.onReceive = onReceive;
            stream = this.client.GetStream();
            id = ++nextId;
        }

        public void Start()
        {
            running = true;
            worker = new Thread(() =>
            {
                try
                {
                    while (running)
                    {
                        while (!stream.DataAvailable)
                            ;
                        while (client.Available < 3)
                            ;
                        var bytes = new byte[client.Available];
                        stream.Read(bytes, 0, bytes.Length);
                        var json = Encoding.UTF8.GetString(bytes);
                        if (Regex.IsMatch(json, "^GET", RegexOptions.IgnoreCase))
                        {
                            HandShake(json);
                        }
                        else
                        {
                            HandleMessage(bytes);
                        }
                    }
                }
                catch
                {
                    running = false;
                }
                Console.WriteLine("Client shutting down");
            });
            worker.Start();
        }

        public void SendData(string data)
        {
            var reply = Encode(data);
            try 
            { 
                stream.Write(reply, 0, reply.Length);
            }
            catch
            {
                Stop();
            }
        }

        public void Stop()
        {
            running = false;
            stream.Close();
            worker.Join();
        }

        private void HandShake(string json)
        {
            var swk = Regex.Match(json, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
            var swka = $"{swk}258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            var swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
            var swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

            var pcol = "HTTP/1.1 101 Switching Protocols\r\n";
            var con = "Connection: Upgrade\r\n";
            var opp = "Upgrade: websocket\r\n";
            var sha = $"Sec-WebSocket-Accept: {swkaSha1Base64}\r\n\r\n";
            var res = Encoding.UTF8.GetBytes($"{pcol}{con}{opp}{sha}");

            stream.Write(res, 0, res.Length);
        }

        private void HandleMessage(byte[] bytes)
        {
            var fin = (bytes[0] & 0b10000000) != 0;
            var mask = (bytes[1] & 0b10000000) != 0;
            var optCode = bytes[0] & 0b00001111;
            var msgLen = bytes[1] - 128;
            var offset = 2;

            if (optCode == 8)
            {
                running = false;
            }
            else if (optCode == 1)
            {
                var decoded = new byte[msgLen];
                var masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                offset += 4;
                for (var i = 0; i < msgLen; ++i)
                {
                    decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);
                }

                var msg = Encoding.UTF8.GetString(decoded);
                onReceive(id, msg);
            }
        }

        private byte[] Encode(string json)
        {
            var data = Encoding.UTF8.GetBytes(json);
            var frame = new byte[10];
            frame[0] = 129;
            var framesize = 2;
            int len = data.Length;
            if (len < 126)
            {
                frame[1] = (byte)len;
            }
            else if (len >= 126 && len <= 65535)
            {
                frame[1] = 126;
                frame[2] = (byte)((len >> 8) & 255);
                frame[3] = (byte)(len & 255);
                framesize = 4;
            }
            else
            {
                frame[1] = 127;
                frame[2] = (byte)((len >> 56) & 255);
                frame[3] = (byte)((len >> 48) & 255);
                frame[4] = (byte)((len >> 40) & 255);
                frame[5] = (byte)((len >> 32) & 255);
                frame[6] = (byte)((len >> 24) & 255);
                frame[7] = (byte)((len >> 16) & 255);
                frame[8] = (byte)((len >> 8) & 255);
                frame[9] = (byte)(len & 255);
                framesize = 10;
            }
            var length = framesize + len;
            var reply = new byte[length];
            var limit = 0;
            for (var i = 0; i < framesize; ++i)
            {
                reply[limit] = frame[i];
                ++limit;
            }
            for (var i = 0; i < len; ++i)
            {
                reply[limit] = data[i];
                ++limit;
            }
            return reply;
        }
    }
}
