using System.Net;
using System.Net.Sockets;
using ParserServ;

var port = 8080;
var ip = "127.0.0.1";

var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
try
{
   
    listener.Start();
    Console.WriteLine("Ожидание подключений...");

    while (true)
    {
        TcpClient client = listener.AcceptTcpClient();
        TcpServer clientObject = new TcpServer(client);

        // создаем новый поток для обслуживания нового клиента
        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
        clientThread.Start();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    if (listener != null)
        listener.Stop();
}
