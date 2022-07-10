﻿using System.Data.SqlClient;
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
        var command = new SqlCommand($@"INSERT INTO Moex (SecID,Board) VALUES ('{secID}','{board}')", connection)
            .ExecuteNonQuery();
        connection.Close();
    }


    public void Start(int ms)
    {
        while (true)
        {
            // var connectionReading =
            //     new SqlConnection(
            //         @"Server=sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345");
            // var connectionWriting =
            //     new SqlConnection(
            //         @"Server=sql.bsite.net\MSSQL2016;Persist Security Info=True;User ID=metallplaceproject_SampleDB;Password=12345");
            //
            // connectionReading.Open();
            // connectionWriting.Open();


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
                        var a = TakeData(reader.GetValue(1).ToString().Trim(), reader.GetValue(2).ToString().Trim());


                        var command =
                            new SqlCommand(
                                    $@"INSERT INTO MoexDataAll (SecIDNum, ParsingDate, DataMOEX, LastPrice)
                      VALUES ({int.Parse(reader.GetValue(0).ToString().Trim())},'{a.Item2}','{a.Item3}',{a.Item4})",
                                    connectionWriting)
                                .ExecuteNonQuery();
                    }
                }
            }


            // connectionReading.Close();
            // connectionWriting.Close();

            Console.WriteLine("Sleeping. . .");
            Thread.Sleep(ms); // 1 min = 60000 ms
        }
    }


    private (string, string, string, string) TakeData(string? secid, string? board)
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