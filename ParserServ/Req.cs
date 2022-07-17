using System.Net.Sockets;
using OpenQA.Selenium.DevTools.V103.Page;

namespace ParserServ;

public class Req
{
    public string DoStatus(string name, string status)
    {
        switch (status)
        {
            case "start":
            {
                foreach (var variable in Program.Tasks)
                {
                    if (variable.Key == name)
                    {
                        return "exists";
                    }
                }

                return "not exists";
            }
            case "stop":
            {
                foreach (var variable in Program.Tasks)
                {
                    if (variable.Key == name)
                    {
                        Program.TaskStop[name] = true;
                        return "aborted";
                    }
                }

                return "there is no working thread";
            }
            default:
                Console.WriteLine("Invalid status");
                break;
        }

        return "Invalid";
    }


    public void SRequest(string name, DateTime dateStart, DateTime dateEnd, int intervalMs)
    {
        Program.TaskStop[name] = false;
        switch (name)
        {
            case "moex":


                CheckDateForWait(dateStart);

                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new Moex().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                Console.WriteLine("Закончил moex");
                break;

            case "mcena":
                CheckDateForWait(dateStart);

                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new McenaParser().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }
                Console.WriteLine("Закончил mcena");
                break;

            case "t_economics":
                CheckDateForWait(dateStart);
                
                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new T_economics().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }
                
                Console.WriteLine("Закончил t_economics");
                break;

            case "translom":

                CheckDateForWait(dateStart);

                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new TranslomParse().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                break;

            default:
                Console.WriteLine($"Did not find this.. how did you say?  - {name}");
                break;
        }
    }

    private void CheckDateForWait(DateTime dateStart)
    {
        if (dateStart > DateTime.Now)
        {
            Thread.Sleep((int)dateStart.Subtract(DateTime.Now).TotalMilliseconds); //Wait until start
        }
    }
}