using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserServ.SqlAccess;

namespace ParserServ.InternetParser
{
    internal class MoexParser : IParser<string>
    {
        public MoexParser()
        {

        }
        public List<string> Load()
        { 
            SqlCrud sql=new SqlCrud();           
            List<string> moexCompanies=sql.GetMoexCompanies();
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.moex.com/");
            return  moexCompanies;
            
        }

        public void Send()
        {
            throw new NotImplementedException();
        }

    }
}
