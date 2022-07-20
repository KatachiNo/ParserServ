using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Sockets;
using System.Xml.Linq;
using Npgsql;

namespace ParserServ;

public class Moex
{
    private readonly string address = "Host=79.133.181.109;Username=postgres;Password=hBYqzt}9S#n2@Pv;Database=postgres";

    public void AddStock(string secID, string board, string name, NetworkStream stream)
    {
        try
        {
            ParseProcess(secID, board);
        }
        catch
        {
            Program.MsgSendAndWrite($"Stock was not added since Moex does not give information about it", stream);
            return;
        }

        var connection = new NpgsqlConnection(address);
        connection.Open();
        new NpgsqlCommand($@"INSERT INTO moex (secid,board,name) VALUES ('{secID}','{board}','{name}')",
                connection)
            .ExecuteNonQuery();
        connection.Close();

        Program.MsgSendAndWrite($"Stock was added succesfully", stream);
    }

    public void DeleteStock(string secID, NetworkStream stream)
    {
        using (var connectionReading = new NpgsqlConnection(address))
        {
            using (var connectionWriting = new NpgsqlConnection(address))
            {
                connectionReading.Open();
                connectionWriting.Open();
                var reader = new NpgsqlCommand("SELECT * FROM moex", connectionReading).ExecuteReader();
                while (reader.Read())
                {
                    var value1 = reader.GetValue(1).ToString()?.Trim();
                    if (value1 == secID)
                    {
                        try
                        {
                            new NpgsqlCommand($@"Delete from moex where secid = '{secID}'", connectionWriting)
                                .ExecuteNonQuery();
                        }
                        catch
                        {
                            Program.MsgSendAndWrite($"Stock was not removed. {secID} has connections with another DB", stream);
                            break;
                        }
                        
                        Program.MsgSendAndWrite($"Stock was removed", stream);
                        break;
                    }
                }

                Program.MsgSendAndWrite($"Stock was not removed. {secID} was not found in moex table", stream);
            }
        }
    }

    public void Start(NetworkStream stream)
    {
        using (var connectionReading = new NpgsqlConnection(address))
        {
            using (var connectionWriting = new NpgsqlConnection(address))
            {
                connectionReading.Open();
                connectionWriting.Open();
                var reader = new NpgsqlCommand("SELECT * FROM moex", connectionReading).ExecuteReader();

                while (reader.Read())
                {
                    var value1 = reader.GetValue(1).ToString()?.Trim();
                    var value2 = reader.GetValue(2).ToString()?.Trim();
                    try
                    {
                        var a = ParseProcess(value1, value2);


                        new NpgsqlCommand(
                                $@"INSERT INTO moexdataall (secidNum, parsingdate, datamoex, lastprice)
                            VALUES ({int.Parse(reader.GetValue(0).ToString().Trim())},'{a.Item2}','{a.Item3}',{a.Item4})",
                                connectionWriting)
                            .ExecuteNonQuery();
                    }
                    catch
                    {
                        var msg = $"{value1} {value2}/Ошибка. Мосбиржа не передала данные от акции";
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