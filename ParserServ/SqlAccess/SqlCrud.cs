using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserServ.SqlAccess
{
    internal class SqlCrud
    {
        private readonly string _connectionString;
        private SqliteDataAccess _dataAccess;
        public SqlCrud()
        {
            _dataAccess = new SqliteDataAccess();
            _connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        }
       
        public void AddToEconomics(string name,string price)
        {
            DateTime date = DateTime.Now;
            string sql = " IF NOT EXISTS ( select Id From EconomicsTable Where ProductName=@name) begin insert into EconomicsTable values(@name) end";
            _dataAccess.SaveData(sql, new {name},_connectionString);
            sql = "insert into EconomicsParse values((select Id from EconomicsTable where ProductName=@name),@price,@date)";
            _dataAccess.SaveData(sql, new { name,price,date }, _connectionString);
        }
    }
}
