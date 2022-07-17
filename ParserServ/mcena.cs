using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParserServ.SqlAccess;

public class McenaParser
{
    List<string[]> urls;
    static SqlCrud sql;

    public McenaParser()
    {
        sql = new SqlCrud();
        urls = new List<string[]>()
        {
            new string[] { "1", "https://www.mcena.ru/metalloprokat/armatura" },
            new string[] { "2", "https://www.mcena.ru/metalloprokat/katanka" },
            new string[] { "1", "https://www.mcena.ru/metalloprokat/kvadrat" },
            new string[] { "1", "https://www.mcena.ru/metalloprokat/krug" },
            new string[] { "1", "https://www.mcena.ru/metalloprokat/list" },
            new string[] { "2", "https://www.mcena.ru/metalloprokat/list" },
            new string[] { "3", "https://www.mcena.ru/metalloprokat/list" },
            new string[] { "4", "https://www.mcena.ru/metalloprokat/list" },
            new string[] { "5", "https://www.mcena.ru/metalloprokat/list" }
        };
    }

    public void Start()
    {
        var id = 1;
        foreach (var u in urls)
        {
            Pars(u[1], int.Parse(u[0]), id);
            id++;
        }
    }

    public void Pars(string url, int line, int id)
    {
        ChromeOptions option = new ChromeOptions();
        option.AddArgument("headless");
        IWebDriver driver = new ChromeDriver(option);
        driver.Navigate().GoToUrl(url);
        string q =
            "body > main > div.main__content > article:nth-child(3) > div > div > div > div > table > tbody > tr:nth-child(" +
            line.ToString() + ") > td:nth-child(2)";
        var v = driver.FindElement(By.CssSelector(q));
        string s = v.Text.Replace(" ", "");
        decimal res = (decimal)(0.8 * int.Parse(s));
        sql.AddMcenaData(id, res, DateTime.Now);
    }
}