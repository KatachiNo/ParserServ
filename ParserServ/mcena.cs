using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class McenaParser
{
    public McenaParser()
    {
        string u = "https://www.mcena.ru/metalloprokat/list/goryachekatanyj_ceny";
        Pars(u);
    }
    public void Pars(string url)
    {
        IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(url);

        //тут проблема с кнопкой
        /*
        //var b = driver.FindElement(By.CssSelector("body > main > div > div.main__content > div.prices__block.prices__no-bottom > div.price-table__button"));
        while (true)
        {
            try
            {
                driver.FindElement(By.CssSelector("body > main > div > div.main__content > div.prices__block.prices__no-bottom > div.price-table__button")).Click();
            }
            catch
            {
                break;
            }
        }
        */

        List<int> prices = new List<int>();
        var f = driver.FindElement(By.ClassName("prices__body"));
        var v = f.FindElements(By.TagName("td"));
        foreach (var item in v)
        {
            string s = item.Text.Replace(" ", "");
            int res;
            if (Int32.TryParse(s, out res))
                prices.Add(res);      
        }

        int sum = 0;
        foreach (var p in prices)
            sum = sum + p;
        double r = sum / prices.Count;
        Console.WriteLine(r);
    }
}
