using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class McenaParser
{
    public McenaParser()
    {
        string u = "https://www.mcena.ru/metalloprokat/krug/krug-gost-2590_ceny";
        Pars(u);

    }
    public void Pars(string url)
    {
        IWebDriver driver = new ChromeDriver();
        driver.Navigate().GoToUrl(url);
        //var b = driver.FindElement(By.CssSelector("body > main > div > div.main__content > div.prices__block.prices__no-bottom > div.price-table__button"));
        while (1 != 0)
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
        /*
        var f = driver.FindElements(By.CssSelector("body > main > div > div.main__content > div.prices__block.prices__no-bottom > div.prices__wrapper"));
        foreach (var item in f)
        {
            Console.WriteLine(item.Text + "!");
        }
        */
    }
}
