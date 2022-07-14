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

        public void AddInTranslom(string id, string MetallType, string AreaType, string AreaName, string Price, string Date, string ParseDate)
        {
            string sql = " insert into Translom values(@id,@MetallType,@AreaType,@AreaName,@Price,@Date,@ParseDate);";
            _dataAccess.SaveData(sql, new { id, MetallType, AreaType, AreaName, Price, Date, ParseDate}, _connectionString);
        }
    }
}
