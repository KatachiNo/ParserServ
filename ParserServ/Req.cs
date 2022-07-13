namespace ParserServ;

public class Req
{
    public void Requ(string name, DateTime dateStart, DateTime dateEnd, int intervalMs)
    {
        switch (name)
        {
            case "moex":

                Thread.Sleep((int)DateTime.Now.Subtract(dateStart).TotalMilliseconds); //Wait until start
                while (DateTime.Now < dateEnd)
                {
                    new Moex().Start();
                    Console.WriteLine("Sleeping. . .");
                    Thread.Sleep(intervalMs); // 1 min = 60000 ms
                }

                break;

            case "mcena":
                Console.WriteLine("got it mcena");
                break;

            case "t_economics":
                Console.WriteLine("got it t_economics");
                break;

            default:
                Console.WriteLine($"Did not find this {name}");
                break;
        }
    }
}