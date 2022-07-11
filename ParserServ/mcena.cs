using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class McenaParser
{
    public McenaParser()
    {
        List<string[]> urls = new List<string[]>();
        urls.Add(new string[] { "1", "https://www.mcena.ru/metalloprokat/armatura" });
        urls.Add(new string[] { "2", "https://www.mcena.ru/metalloprokat/katanka" });
        urls.Add(new string[] { "1", "https://www.mcena.ru/metalloprokat/kvadrat" });
        urls.Add(new string[] { "1", "https://www.mcena.ru/metalloprokat/krug" });
        urls.Add(new string[] { "1", "https://www.mcena.ru/metalloprokat/list" });
        urls.Add(new string[] { "2", "https://www.mcena.ru/metalloprokat/list" });
        urls.Add(new string[] { "3", "https://www.mcena.ru/metalloprokat/list" });
        urls.Add(new string[] { "4", "https://www.mcena.ru/metalloprokat/list" });
        urls.Add(new string[] { "5", "https://www.mcena.ru/metalloprokat/list" });
        int id = 1;
        foreach (string[] url in urls)
        {
            Pars(url[1], Int32.Parse(url[0]), id);
            id++;
        }
    }
    public void Pars(string url, int line, int id)
    {
        ChromeOptions option = new ChromeOptions();
        option.AddArgument("headless");
        IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(url);
        string q = "body > main > div.main__content > article:nth-child(3) > div > div > div > div > table > tbody > tr:nth-child(" + line.ToString() + ") > td:nth-child(2)";
        var v = driver.FindElement(By.CssSelector(q));
        string s = v.Text.Replace(" ", "");
        int res = Int32.Parse(s);
        Console.WriteLine(id + " " + res);        
    }
}
