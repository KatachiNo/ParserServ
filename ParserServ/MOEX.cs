using System.Data.SqlClient;
using System.Xml.Linq;

namespace ParserServ;

public class Moex
{
    public void AddStock(string secID, string board)
    {
        var connection =
            new SqlConnection(
                @"Server=sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345");
        connection.Open();
        var command = new SqlCommand($@"INSERT INTO Moex (SecID,Board) VALUES ('{secID}','{board}')",connection).ExecuteNonQuery();
        connection.Close();
    }


    public (string, string, string, string) TakeData(string board, string secid)
    {
        var xml = XDocument.Load(
            $@"https://iss.moex.com/iss/engines/stock/markets/shares/boards/{board}/securities/{secid}/.xml?iss.meta=off");

        var lastPrice = xml.Elements("document").Elements("data")
            .FirstOrDefault(x => x.Attribute("id")?.Value == "marketdata")!
            .Element("rows")
            ?.Element("row")!.Attribute("LAST");

        var timeMoex_temp = xml.Elements("document").Elements("data")
            .FirstOrDefault(x => x.Attribute("id")?.Value == "marketdata")!
            .Element("rows")
            ?.Element("row")!.Attribute("TIME").Value;

        var prevdate = xml.Elements("document").Elements("data")
            .FirstOrDefault(x => x.Attribute("id")?.Value == "securities")!
            .Element("rows")
            ?.Element("row")!.Attribute("PREVDATE").Value.ToString();

        var t = DateTime.Parse(timeMoex_temp);
        var fullData = DateTime.Parse(prevdate).AddDays(1).Add(new TimeSpan(t.Hour, t.Minute, t.Second)).ToString();

        Console.WriteLine(
            $"Акция {secid} последняя цена {lastPrice.Value} время парсинга {DateTime.Now} Время по MOEX {fullData}");

        return (secid, DateTime.Now.ToString(), fullData, lastPrice.Value);
    }
}