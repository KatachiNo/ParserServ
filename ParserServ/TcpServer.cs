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
                // отправляем обратно сообщение в верхнем регистре
                //var message1 = $"Твое сообщение :{message}: получено :)";
                //data = Encoding.UTF8.GetBytes(message1);
                //stream.Write(data, 0, data.Length);

                var re = message.Split("/");

                new Thread(() =>
                {
                    r.Requ(re[0], DateTime.Parse(re[1]), DateTime.Parse(re[2]), int.Parse(re[3]));
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