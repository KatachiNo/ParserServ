using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using ParserServ.SqlAccess;

namespace ParserServ;

public class TranslomParse
{
    private List<List<string>> GetFromSite()
    {
        var translist = new List<List<string>>();
        var areas = new List<string>();
        var prices = new List<string>();
        var data = new List<string>();

        var chromeOptions = new ChromeOptions();
        chromeOptions.BrowserVersion = "103";
        var driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4444/wd/hub"), chromeOptions);


        driver.Navigate().GoToUrl("https://www.translom.ru/graph/index.html#graph41");
        var elementAl = driver.FindElement(By.XPath("/html/body/section/div/div[1]/ul/li[2]/div/div[1]"));
        elementAl.Click();
        var elementCu = driver.FindElement(By.XPath("/html/body/section/div/div[1]/ul/li[3]/div/div[1]"));
        elementCu.Click();

        foreach (var item in driver.FindElements(By.ClassName("statistic__radio-el")))
        {
            areas.Add(item.FindElement(By.TagName("label")).Text);
        }

        foreach (var item in driver.FindElements(By.ClassName("statistic__right-data")))
        {
            prices.Add(item.Text.Replace("\r\n", ""));
        }

        foreach (var item in driver.FindElements(By.ClassName("statistic__data")))
        {
            if (item.Text != "")
            {
                data.Add(item.Text);
            }
        }

        for (var i = 0; i < prices.Count; i++)
        {
            switch (i)
            {
                case < 4:
                    translist.Add(new List<string> { "Лом черных металлов", "NULL", areas[i], prices[i], data[0] });
                    break;
                case >= 4 and < 13:
                    translist.Add(new List<string>
                        { "Лом черных металлов", "Внутренний рынок", areas[i], prices[i], data[0] });
                    break;
                case > 13 and < 16:
                    translist.Add(new List<string>
                        { "Лом черных металлов", "Внешний рынок", areas[i], prices[i], data[0] });
                    break;
                case 16:
                    translist.Add(new List<string>
                        { "Алюминий смешанный", "Внутренний рынок", "FCA", prices[i], data[1] });
                    break;
                case 18:
                    translist.Add(new List<string> { "Медь 3 сорт", "Внутренний рынок", "FCA", prices[i], data[2] });
                    break;
            }
        }

        driver.Quit();
        return translist;
    }

    // public void SaveTypes()
    // {
    //     var sql = new SqlCrud();
    //     var t = GetFromSite();
    //     for (var i = 0; i < t.Count; i++)
    //     {
    //         sql.AddInTranslom( t[i][0], t[i][1], t[i][2]);
    //     }
    // }
    public void Start()
    {
        var sql = new SqlCrud();
        var t = GetFromSite();
        for (var i = 0; i < t.Count; i++)
        {
            sql.AddInTranslomParse((i + 1), t[i][3], t[i][4], DateTime.Now);
        }
    }
}