using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ParserServ.SqlAccess;

namespace ParserServ;

public class TranslomParse
{
    public List<List<string>> GetFromSite()
    {
        List<List<string>> translist = new List<List<string>>();
        List<string> areas = new List<string>();
        List<string> prices = new List<string>();
        List<string> data = new List<string>();
        ChromeOptions opt = new ChromeOptions();
        opt.AddArgument("headless");
        IWebDriver driver = new ChromeDriver(opt);
        driver.Navigate().GoToUrl("https://www.translom.ru/graph/index.html#graph41");
        var elementAl = driver.FindElement(By.XPath("/html/body/section/div/div[1]/ul/li[2]/div/div[1]"));
        elementAl.Click();
        var elementCu = driver.FindElement(By.XPath("/html/body/section/div/div[1]/ul/li[3]/div/div[1]"));
        elementCu.Click();
        var a = driver.FindElements(By.ClassName("statistic__radio-el"));
        foreach (var item in a)
        {
            areas.Add(item.FindElement(By.TagName("label")).Text);
        }
        var p = driver.FindElements(By.ClassName("statistic__right-data"));
        foreach (var item in p)
        {
            prices.Add(item.Text.Replace("\r\n", ""));
        }
        var d = driver.FindElements(By.ClassName("statistic__data"));
        foreach (var item in d)
        {
            if(item.Text!="")
            {data.Add(item.Text);}
        }
        for (int i=0; i<prices.Count; i++)
        {
            if (i < 4)
            {
                translist.Add(new List<string>{"Лом черных металлов", "NULL", areas[i], prices[i], data[0], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")});
            }
            if ((i>=4)&&(i<13))
            {
                translist.Add(new List<string>{"Лом черных металлов", "Внутренний рынок", areas[i], prices[i], data[0], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")});
            }
            if ((i > 13)&&(i<16))
            {
                translist.Add(new List<string>{"Лом черных металлов", "Внешний рынок", areas[i], prices[i], data[0], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")});
            }

            if (i == 16)
            {
                translist.Add(new List<string>{"Алюминий смешанный", "Внутренний рынок", "FCA", prices[i], data[1], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")});
            }

            if (i == 18)
            {
                translist.Add(new List<string>{"Медь 3 сорт", "Внутренний рынок", "FCA", prices[i], data[2], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")});
            }
        }
        return translist;
    }

    public void SaveTypes()
    {
        SqlCrud sql = new SqlCrud();
        List<List<string>> t = GetFromSite();
        for (int i = 0; i < t.Count; i++)
        {
            sql.AddInTranslom( t[i][0], t[i][1], t[i][2]);
        }
    }
    public void SendTranslomInBase()
    {
        SqlCrud sql = new SqlCrud();
        List<List<string>> t = GetFromSite();
        for (int i = 0; i < t.Count; i++)
        {
            sql.AddInTranslomParse((i+1), t[i][3], t[i][4], t[i][5]);
        }
    }
}