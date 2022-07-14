using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronXL;
using System.IO.Compression;
using IronXL.Styles;

namespace ParserServ;

public class excel
{
    List<string> files;
    public excel()
    {
        p();
    }
    public void p()
    {
        if (File.Exists("price.zip"))
        {
            File.Delete("price.zip");
        }
        string link = "https://www.oreht.ru/price/price.zip";
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(link, "price.zip");
        }
        string path = Environment.CurrentDirectory + @"\эксель";
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            file.Delete();
        }
        ZipFile.ExtractToDirectory("price.zip", path);
        files = new List<string>();
        foreach (FileInfo file in dirInfo.GetFiles())
        {
            if ((file.Name.StartsWith('0')) || (file.Name.StartsWith('1')) || (file.Name.StartsWith('2')))
                files.Add(file.FullName);
        }
        foreach (string file in files)
        {
            WorkBook wb = WorkBook.Load(file);
            WorkSheet ws = wb.GetWorkSheet("Лист 1");
            List<List<string>> res = new List<List<string>>();
            List<string> caterogy = new List<string>();
            caterogy.Add(ws["B79"].ToString());
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
                else
                {
                    sublist.Add(currentcat);
                    res.Add(sublist);
                }
            }
            /*
            //проверка
            foreach (var item in res)
            {
                foreach (var v in item)
                    Console.Write(v + "|");
                Console.WriteLine();
                Console.WriteLine();
            }
            break;
            */
        }
    }
}
