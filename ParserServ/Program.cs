using System.Data.SqlClient;
using ParserServ;

var moex = new Moex();

var connection =
    new SqlConnection(
        @"Server=sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345");
await connection.OpenAsync();







while (true)
{
    var a = moex.TakeData("TQBR", "AMEZ");
    var b = moex.TakeData("TQBR", "IGST");
    var c = moex.TakeData("TQBR", "IGSTP");
    var d = moex.TakeData("TQBR", "KMEZ");


    var commanda = new SqlCommand();
    var commandb = new SqlCommand();
    var commandc = new SqlCommand();
    var commandd = new SqlCommand();


    commanda.CommandText =
        $@"INSERT INTO MoexDataAll (SecIDNum, ParsingDate, DataMOEX, LastPrice) VALUES (1,'{a.Item2}','{a.Item3}',{a.Item4})";

    commandb.CommandText =
        $@"INSERT INTO MoexDataAll (SecIDNum, ParsingDate, DataMOEX, LastPrice) VALUES (4,'{b.Item2}','{b.Item3}',{b.Item4})";

    commandc.CommandText =
        $@"INSERT INTO MoexDataAll (SecIDNum, ParsingDate, DataMOEX, LastPrice) VALUES (3,'{c.Item2}','{c.Item3}',{c.Item4})";

    commandd.CommandText =
        $@"INSERT INTO MoexDataAll (SecIDNum, ParsingDate, DataMOEX, LastPrice) VALUES (5,'{d.Item2}','{d.Item3}',{d.Item4})";

    commanda.Connection = connection;
    commandb.Connection = connection;
    commandc.Connection = connection;
    commandd.Connection = connection;
    commanda.ExecuteNonQuery();
    commandb.ExecuteNonQuery();
    commandc.ExecuteNonQuery();
    commandd.ExecuteNonQuery();

    Console.WriteLine("Sleeping. . .");
    Thread.Sleep(60000); // 1 min = 60000 ms
    
}

