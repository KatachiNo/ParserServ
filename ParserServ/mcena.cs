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
            new[] { "1", "https://www.mcena.ru/metalloprokat/armatura" },
            new[] { "2", "https://www.mcena.ru/metalloprokat/katanka" },
            new[] { "1", "https://www.mcena.ru/metalloprokat/kvadrat" },
            new[] { "1", "https://www.mcena.ru/metalloprokat/krug" },
            new[] { "1", "https://www.mcena.ru/metalloprokat/list" },
            new[] { "2", "https://www.mcena.ru/metalloprokat/list" },
            new[] { "3", "https://www.mcena.ru/metalloprokat/list" },
            new[] { "4", "https://www.mcena.ru/metalloprokat/list" },
            new[] { "5", "https://www.mcena.ru/metalloprokat/list" }
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

    private void Pars(string url, int line, int id)
    {
        var option = new ChromeOptions();
        option.AddArgument("headless");
        var driver = new ChromeDriver(option);
        driver.Navigate().GoToUrl(url);

        var v = driver.FindElement
        (By.CssSelector(
            "body > main > div.main__content > article:nth-child(3) > div > div > div > div > table > tbody > tr:nth-child(" +
            line + ") > td:nth-child(2)"));

        var res = 0.8m * decimal.Parse(v.Text.Replace(" ", ""));
        sql.AddMcenaData(id, res, DateTime.Now);
    }
}