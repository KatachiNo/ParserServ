using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Sockets;
using System.Xml.Linq;

namespace ParserServ;

public class Moex
{
    public void AddStock(string secID, string board, string name)
    {
        var connection =
            new SqlConnection(
                @"Server=sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345");
        connection.Open();
        var command = new SqlCommand($@"INSERT INTO Moex (SecID,Board,Name) VALUES ('{secID}','{board}','{name}')",
                connection)
            .ExecuteNonQuery();
        connection.Close();
    }


    public void Start(NetworkStream stream)
    {
        using (var connectionReading =
               new SqlConnection(
                   @"Server=sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345"))
        {
            using (var connectionWriting =
                   new SqlConnection(
                       @"Server=sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345"))
            {
                connectionReading.Open();
                connectionWriting.Open();
                var reader = new SqlCommand("SELECT * FROM Moex", connectionReading).ExecuteReader();

                while (reader.Read())
                {
                    var value1 = reader.GetValue(1).ToString()?.Trim();
                    var value2 = reader.GetValue(2).ToString()?.Trim();
                    try
                    {
                        var a = ParseProcess(value1, value2);

                        var command =
                            new SqlCommand(
                                    $@"INSERT INTO MoexDataAll (SecIDNum, ParsingDate, DataMOEX, LastPrice)
                            VALUES ({int.Parse(reader.GetValue(0).ToString().Trim())},'{a.Item2}','{a.Item3}',{a.Item4})",
                                    connectionWriting)
                                .ExecuteNonQuery();
                    }
                    catch
                    {
                        var msg = $"{value1} {value2}/Ошибка. Мосбиржа не передала данные от акции ";
                        Program.MsgSendAndWrite(msg, stream);
                        Console.WriteLine(msg);
                    }
                }
            }
        }
    }


    private (string? secid, string dataNow, string dataMoex, string Value) ParseProcess(string? secid, string? board)
    {
        DateTime fullData;

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

        var fullData_temp = DateTime.Parse(prevdate).Add(new TimeSpan(t.Hour, t.Minute, t.Second));
        var t2 = fullData_temp.DayOfWeek.ToString();

        if (t2 == "Friday")
        {
            fullData = fullData_temp.AddDays(3);
        }
        else
        {
            fullData = fullData_temp.AddDays(1);
        }

        var dataNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var dataMoex = fullData.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Console.WriteLine(
            $"Акция {secid} последняя цена {lastPrice.Value} время парсинга {dataNow} Время по MOEX {dataMoex}");

        return (secid, dataNow, dataMoex, lastPrice.Value);
    }
}