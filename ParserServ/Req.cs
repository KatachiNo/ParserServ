using System.Net.Sockets;
using System.Xml.Linq;
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
                    if (variable.Item1 == name)
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
                    if (variable.Item1 == name)
                    {
                        Program.TaskStop[name] = true;
                        return "aborted";
                    }
                }

                return "there is no working thread";
            }
            case "status":
            {
                foreach (var variable in Program.Tasks)
                {
                    if (variable.Item1 == name)
                    {
                        return $"exists/Name={variable.Item1}/DStart={variable.Item3}/DStart={variable.Item4}";
                    }
                }

                return "not exists";
            }
            default:
                Console.WriteLine("Invalid status");
                break;
        }

        return "Invalid";
    }


    public void SRequest(string name, DateTime dateStart, DateTime dateEnd, int intervalMs,NetworkStream stream)
    {
        Program.TaskStop[name] = false;
        switch (name)
        {
            case "moex":
                CheckDateForWait(dateStart);

                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new Moex().Start(stream);
                    Console.WriteLine("Sleeping. . .");
                        //if (Program.TaskStop[name]) break;
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                RemoveTask(name);
                Console.WriteLine("Закончил moex");
                break;

            case "mcena":
                CheckDateForWait(dateStart);

                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new McenaParser().Start();
                    Console.WriteLine("Sleeping. . .");
                    //if (Program.TaskStop[name]) break;
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                RemoveTask(name);
                Console.WriteLine("Закончил mcena");
                break;

            case "t_economics":
                CheckDateForWait(dateStart);

                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new T_economics().Start();
                    Console.WriteLine("Sleeping. . .");
                    //if (Program.TaskStop[name]) break;
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                RemoveTask(name);
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

    private void RemoveTask(string name)
    {
        for (var i = 0; i < Program.Tasks.Count; i++)
        {
            if (Program.Tasks[i].Item1 == name)
            {
                Program.Tasks.RemoveAt(i);
                return;
            }
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