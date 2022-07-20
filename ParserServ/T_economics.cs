using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ParserServ.SqlAccess;

namespace ParserServ
{
    internal class T_economics
    {
        private Dictionary<string, string> _comparedData = new();

        private void LoadData()
        {
            var pattern = @"\b(Нефть|Газ|Бензин|Топочный мазут|Серебро|Золото)\b";
            var options = new ChromeOptions();
            options.AddArgument("headless");

            var driver = new ChromeDriver(options);
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
                    _comparedData.Add(splitedname, value.Text);
                }
            }
        }

        public void Start()
        {
            LoadData();
            var sqlCrud = new SqlCrud();
            foreach (var item in _comparedData)
            {
                sqlCrud.AddToEconomics(item.Key, item.Value);
            }
        }
    }
}