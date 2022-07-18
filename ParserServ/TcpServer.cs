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

                try
                {
                    ConvertOfRequest(builder.ToString(), stream);
                }
                catch
                {
                    var msg = "I don't know what you want";
                    Program.MsgSendAndWrite(msg, stream);
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

    private void ConvertOfRequest(string message, NetworkStream stream)
    {
        var r = new Req();
        var re = message.Split("|", StringSplitOptions.RemoveEmptyEntries);
        // if (re.Length == 0)
        // {
        //     var msg = "I don't know what you want";
        //     Program.MsgSendAndWrite(msg, stream);
        //     return;
        // }

        foreach (var variable in re)
        {
            var res = variable.Split("/", StringSplitOptions.RemoveEmptyEntries);
            switch (res[0])
            {
                case "TakeInfo":
                {
                    var DoResult = r.DoStatus(res[1], "status");
                    var msg = $"{res[1]} {DoResult}";
                    Program.MsgSendAndWrite(msg, stream);
                    return;
                }
                case "WannaStop":
                {
                    var DoResult = r.DoStatus(res[1], "stop");
                    var msg = $"{res[1]} {DoResult}";
                    Program.MsgSendAndWrite(msg, stream);
                    return;
                }
                case "AddStocks":
                {
                    try
                    {
                        new Moex().AddStock(res[1], res[2], res[3]);
                        var msg = $"Stock was added succesful";
                        Program.MsgSendAndWrite(msg, stream);
                        return;
                    }
                    catch
                    {
                        var msg = $"Stock was not added";
                        Program.MsgSendAndWrite(msg, stream);
                        return;
                    }
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
                        int.Parse(res[3]), stream);
                });
                tsk.Start();
                Program.Tasks.Add((res[0], tsk, DateTime.Parse(res[1]), DateTime.Parse(res[1])));
                var msg = "moex was started";
                Program.MsgSendAndWrite(msg, stream);
                break;
            }
            case "exists":
            {
                var msg = $"Task {res[0]} already exists ";
                Program.MsgSendAndWrite(msg, stream);
                break;
            }
            case "aborted":
            {
                var msg = $"Task {res[0]} aborted";
                Program.MsgSendAndWrite(msg, stream);
                break;
            }
            default:
            {
                var msg = $"Invalid start";
                Program.MsgSendAndWrite(msg, stream);
                break;
            }
        }
    }
}