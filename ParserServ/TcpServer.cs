using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ParserServ;

public class TcpServer
{
    public TcpClient clientSocket;

    public void RunClient()
    {
        var readerStream = new StreamReader(clientSocket.GetStream());
        var writeStream = clientSocket.GetStream();
        var returnData = readerStream.ReadLine();
        var username = returnData;
        Console.WriteLine($"Welcome {username} to the Server");

        while (true)
        {
            returnData = readerStream.ReadLine();
            if (returnData.IndexOf("Quit") > -1)
            {
                Console.WriteLine($"Bye bye {username}");
                break;
            }

            Console.WriteLine($"{username} : {returnData}");
            returnData += "\r\n";
            var dataWrite = Encoding.ASCII.GetBytes(returnData);
            writeStream.Write(dataWrite, 0, dataWrite.Length);
        }

        clientSocket.Close();
    }
}