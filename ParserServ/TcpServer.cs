using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ParserServ;

public class TcpServer
{
    private TcpListener server;


    public TcpServer(string ip, int port)
    {
        var localAddress = IPAddress.Parse(ip);

        server = new TcpListener(localAddress, port);
        server.Start();
    }

    public void TcpServerReading()
    {
        try
        {
            while (true)
            {
                var client = server.AcceptTcpClient();
                var stream = client.GetStream();
                Console.WriteLine("Подключен клиент.");

                var data = new byte[256];
                var bytes = stream.Read(data, 0, data.Length);
                Console.WriteLine(Encoding.UTF8.GetString(data, 0, bytes));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}