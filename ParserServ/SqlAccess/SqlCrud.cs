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
    }
}
