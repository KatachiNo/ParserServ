using System.Net.Sockets;
using ParserServ;

var port = 8080;

var listener = new TcpListener(port);
try
{
    listener.Start();
    Console.WriteLine("Waiting connections...");

    while (true)
    {
        var client = listener.AcceptTcpClient();
        // создаем новый поток для обслуживания нового клиента
        new Thread(() => { new TcpServer(client).Process(); }).Start();
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