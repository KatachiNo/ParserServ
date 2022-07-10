// See https://aka.ms/new-console-template for more information

using System.Data.SqlClient;
using ParserServ;

var moex = new Moex();




// using (SqlConnection connection = new SqlConnection(connectionString))
// {
//     await connection.OpenAsync();
//     var command = new SqlCommand();
//     command.CommandText = @"INSERT INTO MoexDataAll (SecID, ParsingDate, DataMOEX, LastPrice) 
//                           VALUES (1,'2007-05-08 12:35:29.1234567','2007-05-08 12:35:29.1234567',32123)";
//     command.Connection = connection;
// }


var connectionString =
    @"sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345;";
var connection = new SqlConnection(connectionString);
connection.Open();
// await connection.OpenAsync();
// var command = new SqlCommand();
// command.CommandText = @"INSERT INTO MoexDataAll (SecID, ParsingDate, DataMOEX, LastPrice) 
//                           VALUES (1,'2007-05-08 12:35:29.1234567','2007-05-08 12:35:29.1234567',32123)";
// command.Connection = connection;

while (true)
{   
    var a = moex.DownloadXml("TQBR", "AMEZ");
    var b = moex.DownloadXml("TQBR", "IGST");
    var c = moex.DownloadXml("TQBR", "IGSTP");
    var d = moex.DownloadXml("TQBR", "KMEZ");
        
    
    await connection.OpenAsync();
    var commanda = new SqlCommand();
    var commandb = new SqlCommand();
    var commandc = new SqlCommand();
    var commandd = new SqlCommand();
   
    
    commanda.CommandText = $@"INSERT INTO MoexDataAll (SecID, ParsingDate, DataMOEX, LastPrice) 
                          VALUES (1,'{a.Item2}','{a.Item3}',{a.Item4})";
    
    commandb.CommandText = $@"INSERT INTO MoexDataAll (SecID, ParsingDate, DataMOEX, LastPrice) 
                          VALUES (4,'{b.Item2}','{b.Item3}',{b.Item4})";
    
    commandc.CommandText = $@"INSERT INTO MoexDataAll (SecID, ParsingDate, DataMOEX, LastPrice) 
                          VALUES (3,'{c.Item2}','{c.Item3}',{c.Item4})";
    
    commandd.CommandText = $@"INSERT INTO MoexDataAll (SecID, ParsingDate, DataMOEX, LastPrice) 
                          VALUES (5,'{d.Item2}','{d.Item3}',{d.Item4})";
    
    commanda.Connection = connection;
    commandb.Connection = connection;
    commandc.Connection = connection;
    commandd.Connection = connection;
    
    
    
    Thread.Sleep(60000); // 1 min = 60000 ms
}