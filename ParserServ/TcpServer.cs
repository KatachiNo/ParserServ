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
                    Program.MsgSendAndWrite("I don't know what you want", stream);
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

        foreach (var variable in re)
        {
            var res = variable.Split("/", StringSplitOptions.RemoveEmptyEntries);
            switch (res[0])
            {
                case "TakeInfo":
                {
                    var DoResult = r.DoStatus(res[1], "status");
                    Program.MsgSendAndWrite($"{res[1]} {DoResult}", stream);
                    return;
                }
                case "WannaStop":
                {
                    var DoResult = r.DoStatus(res[1], "stop");
                    Program.MsgSendAndWrite($"{res[1]} {DoResult}", stream);
                    Program.RemoveTask(res[1]);
                    return;
                }
                case "AddStocks":
                {
                    try
                    {
                        new Moex().AddStock(res[1], res[2], res[3], stream);
                        return;
                    }
                    catch
                    {
                        Program.MsgSendAndWrite("Stock was not added", stream);
                        return;
                    }
                }
                case "DeleteStocks":
                {
                    new Moex().DeleteStock(res[1], stream);
                    break;
                }
                default:
                {
                    try
                    {
                        CheckStatusAndDo(r.DoStatus(res[0], res[4]), res, stream);
                    }
                    catch
                    {
                        Program.MsgSendAndWrite("I don't know what you want", stream);
                    }

                    break;
                }
            }
        }
    }

    private void CheckStatusAndDo(string retReq, string[] res, NetworkStream stream)
    {
        switch (retReq)
        {
            case "not exists":
            {
                var tsk = new Task(() =>
                {
                    new Req().SRequest(res[0], DateTime.Parse(res[1]), DateTime.Parse(res[2]),
                        int.Parse(res[3]), stream);
                });
                tsk.Start();
                Program.Tasks.Add((res[0], tsk, DateTime.Parse(res[1]), DateTime.Parse(res[2]), int.Parse(res[3])));

                break;
            }
            case "exists":
            {
                Program.MsgSendAndWrite($"Task {res[0]} already exists", stream);
                break;
            }
            case "aborted":
            {
                Program.MsgSendAndWrite($"Task {res[0]} aborted", stream);
                break;
            }
            default:
            {
                Program.MsgSendAndWrite("Invalid start", stream);
                break;
            }
        }
    }
}