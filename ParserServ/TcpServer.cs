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
                        var tsk = new Task(() =>
                        {
                            r.Requ(re[0], DateTime.Parse(re[1]), DateTime.Parse(re[2]),
                                int.Parse(re[3]));
                        });
                        tsk.Start();
                        Program.Tasks.Add(re[0], tsk);
                    }
                    else if (retReq == "exists")
                    {
                        var msg = $"Task {re[0]} already exists ";
                        var d = Encoding.Unicode.GetBytes(msg);
                        stream.Write(d, 0, d.Length);
                        Console.WriteLine(msg);
                    }
                    else if (retReq == "Aborted")
                    {
                        var msg = $"Task {re[0]} Aborted";
                        var d = Encoding.Unicode.GetBytes(msg);
                        stream.Write(d, 0, d.Length);
                        Console.WriteLine(msg);
                    }
                    else
                    {
                        var msg = $"Invalid start";
                        var d = Encoding.Unicode.GetBytes(msg);
                        stream.Write(d, 0, d.Length);
                        Console.WriteLine(msg);
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