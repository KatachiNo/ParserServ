using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Net.Sockets;

namespace ParserServ;

public class TcpServer
{
    public TcpClient client;


    public TcpServer(TcpClient tcpClient)
    {
        client = tcpClient;
    }

    public void Process()
    {
        NetworkStream? stream = null;
        var r = new Req();
        try
        {
            stream = client.GetStream();
            var data = new byte[64]; // буфер для получаемых данных

            while (true)
            {
                // получаем сообщение
                var builder = new StringBuilder();
                var bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (stream.DataAvailable);

                var message = builder.ToString();

                var re = message.Split("/");
                if (re.Length > 1)
                {
                    var n = re[0];
                    var retReq = r.PreReq(re[0], re[4]);

                    if (retReq == "not exists")
                    {
                        var tre = new Thread(() =>
                        {
                            r.Requ(re[0], DateTime.Parse(re[1]), DateTime.Parse(re[2]),
                                int.Parse(re[3]));
                        });
                        tre.Name = n;
                        tre.Start();
                        Program.threads.Add(tre);
                    }
                    else if (retReq == "exists")
                    {
                        Console.WriteLine($"Thread {re[0]} already exists ");
                    }
                    else if (retReq == "Aborted")
                    {
                        Console.WriteLine($"Thread {re[0]} Aborted");
                    }
                    else
                    {
                        Console.WriteLine("Invalid start");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            stream?.Close();
            client.Close();
        }
    }
}