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
                        return
                            $"exists/Name={variable.Item1}/DStart={variable.Item3}/DEnd={variable.Item4}/Ms={variable.Item5}";
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


    public void SRequest(string name, DateTime dateStart, DateTime dateEnd, int intervalMs, NetworkStream stream)
    {
        Program.TaskStop[name] = false;
        switch (name)
        {
            case "moex":

                Program.MsgSendAndWrite($"{name} was started", stream);

                CheckDateForWait(dateStart);
                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new Moex().Start(stream);
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                break;

            case "mcena":
                Program.MsgSendAndWrite($"{name} was started", stream);

                CheckDateForWait(dateStart);
                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new McenaParser().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }


                break;

            case "t_economics":
                Program.MsgSendAndWrite($"{name} was started", stream);

                CheckDateForWait(dateStart);
                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new T_economics().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }


                break;

            case "translom":
                Program.MsgSendAndWrite($"{name} was started", stream);

                CheckDateForWait(dateStart);
                while (DateTime.Now < dateEnd && !Program.TaskStop[name])
                {
                    new TranslomParse().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                break;

            default:
                Program.MsgSendAndWrite($"Did not find this.. how did you say?  - {name}", stream);
                Program.RemoveTask(name);
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