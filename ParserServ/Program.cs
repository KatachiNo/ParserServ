using System.Net.Sockets;
using ParserServ;


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