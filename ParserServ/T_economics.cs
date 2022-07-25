using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Remote;
using ParserServ.SqlAccess;

namespace ParserServ
{
    internal class T_economics
    {
        private Dictionary<string, string> _comparedData = new();

        private void LoadData()
        {
            var pattern = @"\b(Нефть|Газ|Бензин|Топочный мазут|Серебро|Золото)\b";
            //var options = new ChromeOptions();
            // options.AddArgument("headless");

            //var driver = new ChromeDriver(options);

            var chromeOptions = new ChromeOptions();
            chromeOptions.BrowserVersion = "103";
            var driver = new RemoteWebDriver(new Uri("http://127.0.0.1:4444/wd/hub"), chromeOptions);

            driver.Navigate().GoToUrl("https://ru.tradingeconomics.com/commodities");
            var row = driver.FindElements(By.ClassName("datatable-row"));
            var alternative = driver.FindElements(By.ClassName("datatable-row-alternating"));

            foreach (var item in row.Union(alternative).ToList())
            {
                var name = item.FindElement(By.ClassName("datatable-item-first"));
                if (Regex.IsMatch(name.Text, pattern, RegexOptions.IgnoreCase))
                {
                    var splitedname = name.Text;
                    var value = item.FindElement(By.Id("p"));
                    _comparedData.Add(splitedname, value.Text.Replace(".", ","));
                }
            }

            driver.Quit();
        }

        public void Start()
        {
            LoadData();
            var sqlCrud = new SqlCrud();
            foreach (var item in _comparedData)
            {
                sqlCrud.AddToEconomics(item.Key, (decimal.Parse(item.Value)));
            }
        }
    }
}