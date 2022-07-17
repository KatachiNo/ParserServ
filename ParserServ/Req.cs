using System.Net.Sockets;

namespace ParserServ;

public class Req
{
    public string PreReq(string name, string status)
    {
        switch (status)
        {
            case "start":
            {
                foreach (var variable in Program.threads)
                {
                    if (variable.Name == name)
                    {
                        return "exists";
                    }
                }

                return "not exists";
            }
            case "stop":
            {
                foreach (var variable in Program.threads)
                {
                    if (variable.Name == name)
                    {
                        variable.Abort();
                        return "Aborted";
                    }
                }

                break;
            }
            default:
                Console.WriteLine("Invalid status");
                break;
        }

        return "Invalid";
    }


    public void Requ(string name, DateTime dateStart, DateTime dateEnd, int intervalMs)
    {
        switch (name)
        {
            case "moex":

                if (dateStart > DateTime.Now)
                {
                    Thread.Sleep((int)dateStart.Subtract(DateTime.Now).TotalMilliseconds); //Wait until start
                }

                while (DateTime.Now < dateEnd)
                {
                    new Moex().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                Console.WriteLine("Закончил moex");
                break;

            case "mcena":
                Console.WriteLine("got it mcena");
                break;

            case "t_economics":
                Console.WriteLine("got it t_economics");
                break;

            case "translom":
                
                if (dateStart > DateTime.Now)
                {
                    Thread.Sleep((int)dateStart.Subtract(DateTime.Now).TotalMilliseconds); //Wait until start
                }

                while (DateTime.Now < dateEnd)
                {
                    new TranslomParse().SendTranslomInBase();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                break;

            default:
                Console.WriteLine($"Did not find this.. how did you say?  - {name}");
                break;
        }
    }
}