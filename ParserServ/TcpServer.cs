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

    [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH",
        MessageId = "type: System.String[]")]
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
                    new Thread(() =>
                    {
                        r.Requ(re[0], DateTime.Parse(re[1]), DateTime.Parse(re[2]),
                            int.Parse(re[3]));
                    }).Start();
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