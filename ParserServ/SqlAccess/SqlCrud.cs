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
        public List<string> GetMoexCompanies()
        {
            string sql="Select CompanyName From MoexCompany";
            return _dataAccess.LoadData<string, dynamic>(sql, new { }, _connectionString);
        }

        public void AddInTranslomParse(int AreaID, string Price, string DateTranslom, string DateParse)
        {
            string sql = "INSERT INTO TranslomParse (AreaID, Price, DateTranslom, DateParse) VALUES (@AreaID, @Price, @DateTranslom, @DateParse)";
            _dataAccess.SaveData(sql, new {AreaID, Price, DateTranslom, DateParse}, _connectionString);
        }
        public void AddInTranslom(string MetallType, string AreaType, string AreaName)
        {
            string sql = "INSERT INTO Translom (MetallType, AreaType, AreaName) VALUES (@MetallType, @AreaType, @AreaName)";
            _dataAccess.SaveData(sql, new {MetallType, AreaType, AreaName}, _connectionString);
        }
        
        
        public void AddMcenaData(int id, decimal res, DateTime date)
        {
            string query = "INSERT INTO McenaPars (Price, ProductID, Date) VALUES (@res, @id, @date)";
            _dataAccess.SaveData(query, new {res, id, date}, _connectionString);
        }
        
        public void AddToEconomics(string name,string price)
        {
            DateTime date = DateTime.Now;
            string sql = " insert into TradingeconomicsTable values(@name,@price,@date);";
            _dataAccess.SaveData(sql, new {name,price,date},_connectionString);
        }
    }
}
