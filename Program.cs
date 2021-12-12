
using WebSockServer;

var server = new Server();
server.Start();

while (server.Running)
{
    Thread.Sleep(1000);
}

Console.WriteLine("All clients disconnected");
server.Stop();

