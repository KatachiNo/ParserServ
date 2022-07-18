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
    internal class Parser
    {
        private Dictionary<string, string> _comparedData = new();
     
        public void  LoadData()
        {
            
            string pattern = @"\b(Нефть|Газ|Бензин|Топочный мазут|Серебро|Золото)\b";
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            WebDriver driver = new ChromeDriver(options);           
            driver.Navigate().GoToUrl("https://ru.tradingeconomics.com/commodities");
            var row = driver.FindElements(By.ClassName("datatable-row"));    
            var alternative= driver.FindElements(By.ClassName("datatable-row-alternating"));
            var finalresult = row.Union(alternative).ToList();
            foreach (var item in finalresult)
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

        public void Parse()
        {
            LoadData();
            SqlCrud sqlCrud = new SqlCrud();
            foreach (var item in _comparedData)
            {
                sqlCrud.AddToEconomics(item.Key,item.Value);
            }
            

        }
        
    }
}
