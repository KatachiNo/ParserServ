using System.Net.Sockets;
using ParserServ;

var  ECHO_PORT = 8080;
var nCLient = 0;

try
{
    var ClientListener = new TcpListener(ECHO_PORT);
    ClientListener.Start();
    Console.WriteLine("Waiting for connections... ");
    while (true)
    {
        var client = ClientListener.AcceptTcpClient();
        var clientHand = new TcpServer();
        clientHand.clientSocket = client;
        var clientThread = new Thread(new ThreadStart(clientHand.RunClient));
        clientThread.Start();
    }

    //ClientListener.Stop();
}
catch (Exception exp)
{
    Console.WriteLine($"Exception {exp}");
}