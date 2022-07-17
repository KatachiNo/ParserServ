using System;
using System.Net.Sockets;


namespace ParserServ
{
    //Сообщение будущему я: Протестировать аборт, добавить возможность нескольких сообщений из одного потока
    internal class Program
    {
        public static Dictionary<string, Task> Tasks = new();

        public static Dictionary<string, bool> TaskStop = new()
        {
            { "moex", false },
            { "mcena", false },
            { "coal", false },
            { "translom", false }
        };

        static void Main()
        {
            //Граница за которую лучше не заходить. Опасная зона
            new Thread(StartServer).Start();

            void StartServer()
            {
                var port = 8080;
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
            //Граница за которую лучше не заходить. Далее... безопасная зона. Выше -> опасная зона. НЕ ТРОГАТЬ.
        }
    }
}