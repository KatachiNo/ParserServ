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


                ConvertOfRequest(builder.ToString(), stream);
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

    private void ConvertOfRequest(string message, NetworkStream stream)
    {
        var r = new Req();
        var re = message.Split("|", StringSplitOptions.RemoveEmptyEntries);
        if (re.Length == 0) return;

        foreach (var variable in re)
        {
            var res = variable.Split("/", StringSplitOptions.RemoveEmptyEntries);
            switch (res[0])
            {
                case "TakeInfo":
                {
                    var DoResult = r.DoStatus(res[1], "start");
                    var msg = $"{res[1]} {DoResult}";
                    MsgSendAndWrite(msg, stream);
                    return;
                }
                case "WannaStop":
                {
                    var DoResult = r.DoStatus(res[1], "stop");
                    var msg = $"{res[1]} {DoResult}";
                    MsgSendAndWrite(msg, stream);
                    return;
                }
                default:
                {
                    CheckStatusAndDo(r.DoStatus(res[0], res[4]), res, stream);
                    break;
                }
            }
        }
    }

    private void CheckStatusAndDo(string retReq, string[] res, NetworkStream stream)
    {
        var r = new Req();
        switch (retReq)
        {
            case "not exists":
            {
                var tsk = new Task(() =>
                {
                    r.SRequest(res[0], DateTime.Parse(res[1]), DateTime.Parse(res[2]),
                        int.Parse(res[3]));
                });
                tsk.Start();
                Program.Tasks.Add(res[0], tsk);
                break;
            }
            case "exists":
            {
                var msg = $"Task {res[0]} already exists ";
                MsgSendAndWrite(msg, stream);
                break;
            }
            case "aborted":
            {
                var msg = $"Task {res[0]} aborted";
                MsgSendAndWrite(msg, stream);
                break;
            }
            default:
            {
                var msg = $"Invalid start";
                MsgSendAndWrite(msg, stream);
                break;
            }
        }
    }

    private void MsgSendAndWrite(string msg, NetworkStream stream)
    {
        var d = Encoding.UTF8.GetBytes(msg);
        stream.Write(d, 0, d.Length);
        Console.WriteLine(msg);
    }
}