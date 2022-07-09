// See https://aka.ms/new-console-template for more information

using ParserServ;

var moex = new Moex();

moex.DownloadXml("TQBR", "AMEZ");
moex.DownloadXml("TQBR", "IGST");
moex.DownloadXml("TQBR", "IGSTP");
moex.DownloadXml("TQBR", "KMEZ");