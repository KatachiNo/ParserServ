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

        public void AddToEconomics(string name,decimal price)
        {
            var date = DateTime.Now;         
            var sql = "insert into EconomicsParse(productid, price, parsingdate) values((select Id from EconomicsTable where ProductName=@name),@price,@date)";
            _dataAccess.SaveData(sql, new { name,price,date }, _connectionString);
        }

        public void AddInTranslomParse(int AreaID, string Price, string DateTranslom, DateTime DateParse)
        {
            var sql = "INSERT INTO TranslomParse (AreaID, Price, DateTranslom, DateParse) VALUES (@AreaID, @Price, @DateTranslom, @DateParse)";
            _dataAccess.SaveData(sql, new {AreaID, Price, DateTranslom, DateParse}, _connectionString);
        }
        public void AddInTranslom(string MetallType, string AreaType, string AreaName)
        {
            var sql = "INSERT INTO Translom (MetallType, AreaType, AreaName) VALUES (@MetallType, @AreaType, @AreaName)";
            _dataAccess.SaveData(sql, new {MetallType, AreaType, AreaName}, _connectionString);
        }
        
        
        public void AddMcenaData(int id, decimal res, DateTime date)
        {
            string query = "INSERT INTO mcenaparse (Price, ProductID, Date) VALUES (@res, @id, @date)";
            _dataAccess.SaveData(query, new {res, id, date}, _connectionString);
        }
    }
}