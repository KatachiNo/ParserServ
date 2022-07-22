using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace ParserServ
{
    internal class Program
    {
        public static List<(string, Task, DateTime, DateTime, int)> Tasks = new();

        public static readonly Dictionary<string, bool> TaskStop = new()
        {
            { "moex", false },
            { "mcena", false },
            { "t_economics", false },
            { "translom", false }
        };

        static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            Console.WriteLine(DateTime.Now);
            
            new Thread(StartServer).Start();

            void StartServer()
            {
                var port = 8081;
                var listener = new TcpListener(port);
                try
                {
                    listener.Start();
                    Console.WriteLine("Waiting connections...");

                    while (true)
                    {
                        var client = listener.AcceptTcpClient();
                        // создаем новый поток для обслуживания нового клиента
                        Console.WriteLine("Ещё один клиент подключился...");
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
            }
            
        }

        public static void MsgSendAndWrite(string msg, NetworkStream stream)
        {
            var d = Encoding.UTF8.GetBytes(msg);
            stream.Write(d, 0, d.Length);
            Console.WriteLine(msg);
        }

        public static void RemoveTask(string name)
        {
            for (var i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].Item1 == name)
                {
                    Tasks.RemoveAt(i);
                    return;
                }
            }
        }
    }
}