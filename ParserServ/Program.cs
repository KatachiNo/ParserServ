﻿// See https://aka.ms/new-console-template for more information

using ParserServ;

var moex = new Moex();
moex.DownloadXml("TQBR", "MAGN");