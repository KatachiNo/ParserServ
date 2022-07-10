using System.Xml.Linq;


namespace ParserServ;

public class Moex
{
    public (string, string, string, string) DownloadXml(string board, string secid)
    {
        var xml = XDocument.Load(
            $@"https://iss.moex.com/iss/engines/stock/markets/shares/boards/{board}/securities/{secid}/.xml?iss.meta=off");

        var lastPrice = xml.Elements("document").Elements("data")
            .FirstOrDefault(x => x.Attribute("id")?.Value == "marketdata")!
            .Element("rows")
            ?.Element("row")!.Attribute("LAST");

        var timeMoex = xml.Elements("document").Elements("data")
            .FirstOrDefault(x => x.Attribute("id")?.Value == "marketdata")!
            .Element("rows")
            ?.Element("row")!.Attribute("TIME");


        Console.WriteLine(
            $"Акция {secid} последняя цена {lastPrice.Value} время парсинга {DateTime.Now} Время по MOEX {timeMoex}");

        return (secid, lastPrice.Value, DateTime.Now.ToString(), timeMoex.ToString());
    }
}