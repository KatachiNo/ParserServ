﻿using System;
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
    }
}
