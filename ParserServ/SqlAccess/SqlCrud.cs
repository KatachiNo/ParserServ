using System.Configuration;

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

        public void AddInTranslom(string id, string MetallType, string AreaType, string AreaName, string Price, string Date, string ParseDate)
        {
            string sql = " insert into Translom values(@id,@MetallType,@AreaType,@AreaName,@Price,@Date,@ParseDate);";
            _dataAccess.SaveData(sql, new { id, MetallType, AreaType, AreaName, Price, Date, ParseDate}, _connectionString);
        }
    }
}
