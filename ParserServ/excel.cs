using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronXL;
using System.IO.Compression;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace ParserServ;

public class excel
{
    StreamWriter sw;
    List<string> files;
    string p;
    void GetArc()
    {
        if (File.Exists("price.zip"))
            File.Delete("price.zip");
        p = Environment.CurrentDirectory + @"\эксель_лог.txt";
        if (!File.Exists(p))
            File.Create(p).Close();
        using (sw = new StreamWriter(p, false))
        {
            sw.WriteLine("Начало парсинга: " + DateTime.Now.ToString());
            sw.WriteLine("Запрос файла: " + DateTime.Now.ToString());
        }
        string link = "https://www.oreht.ru/price/price.zip";
        using (WebClient client = new WebClient())
            client.DownloadFile(link, "price.zip");
        if (File.Exists("price.zip"))
            using (sw = new StreamWriter(p, true))
                sw.WriteLine("Файл получен: " + DateTime.Now.ToString());
        else
            using (sw = new StreamWriter(p, true))
                sw.WriteLine("Файл не получен: " + DateTime.Now.ToString());
    }
    void UnpackArc()
    {
        string path = Environment.CurrentDirectory + @"\эксель";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        foreach (FileInfo file in dirInfo.GetFiles())
            file.Delete();
        using (sw = new StreamWriter(p, true))
            sw.WriteLine("Начало распаковки архива: " + DateTime.Now.ToString());
        ZipFile.ExtractToDirectory("price.zip", path);
        files = new List<string>();
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            if ((file.Name.StartsWith('0')) || (file.Name.StartsWith('1')) || (file.Name.StartsWith('2')))
                files.Add(file.FullName);
        }
        if (files.Count == 0)
            using (sw = new StreamWriter(p, true))
                sw.WriteLine("Архив распакован: " + DateTime.Now.ToString());
        else
            using (sw = new StreamWriter(p, true))
                sw.WriteLine("Архив не распакован: " + DateTime.Now.ToString());
    }
    void ExcelParsing()
    {
        string path = Environment.CurrentDirectory + @"\prices.csv";
        if (!File.Exists(path))
            File.Create(path).Close();
        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { HasHeaderRecord = false, Delimiter = ";", Encoding = Encoding.UTF8 };
        using (sw = new StreamWriter(path, false, Encoding.UTF8))
        using (var csv = new CsvWriter(sw, config))
        {
            csv.WriteField("Артикул");
            csv.WriteField("Название товара");
            csv.WriteField("Единица измерения");
            csv.WriteField("Количество в упаковке");
            csv.WriteField("Цена, руб");
            csv.WriteField("Остаток");
            csv.WriteField("Категория");
            csv.NextRecord();
        }

        foreach (string file in files)
        {
            WorkBook wb = WorkBook.Load(file);
            WorkSheet ws = wb.GetWorkSheet("Лист 1");
            List<record> res = new List<record>();
            List<string> caterogy = new List<string>();
            caterogy.Add(ws["B79"].ToString());
            using (sw = new StreamWriter(p, true))
                sw.WriteLine("Начало обработки файла " + caterogy[0] + ".xls: " + DateTime.Now.ToString());
            int lines = 0;
            for (int i = 79; i < ws.RowCount; i++)
            {
                string currentcat = "";
                foreach (var t in caterogy)
                    currentcat = currentcat + t;
                List<string> sublist = new List<string>();
                bool isit = false;
                foreach (var item in ws[$"B{i}:H{i}"])
                {
                    if (!String.IsNullOrEmpty(item.Text) && !String.IsNullOrWhiteSpace(item.Text))
                        sublist.Add(item.Text);
                    if (item.Style.Font.Italic)
                        isit = true;
                }
                if (sublist.Count == 0)
                    break;
                else if (sublist.Count == 1)
                {
                    if (isit)
                    {
                        caterogy.RemoveRange(1, caterogy.Count - 1);
                        caterogy.Add(sublist[0]);
                    }
                    else
                    {
                        if (caterogy.Count > 2)
                            caterogy.RemoveAt(2);
                        caterogy.Add(sublist[0]);
                    }
                }
                else if (sublist.Count >= 6)
                {
                    if (sublist.Count == 7)
                        sublist.RemoveAt(5);
                    sublist.Add(currentcat);
                    res.Add(new record { art = sublist[0], name = sublist[1], unit = sublist[2], num = sublist[3], price = sublist[4], rem = sublist[5], cat = sublist[6]  });
                }
                lines++;
            }
            using (sw = new StreamWriter(p, true))
                sw.WriteLine("Обработано " + lines + " строк: " + DateTime.Now.ToString());

            using (sw = new StreamWriter(path, true, Encoding.UTF8))
            using (var csv = new CsvWriter(sw, config))
                csv.WriteRecords(res);
        }
    }
    public void Load()
    {
        GetArc();
        UnpackArc();
        ExcelParsing();
    }
}
public class record
{
    public string art { get; set; }
    public string name { get; set; }
    public string unit { get; set; }
    public string num { get; set; }
    public string price { get; set; }
    public string rem { get; set; }
    public string cat { get; set; }
}
