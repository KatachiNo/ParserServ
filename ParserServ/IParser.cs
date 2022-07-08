using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserServ
{
    internal interface IParser<T>
    {
       
        public List<T> Load();
        public void Send();

    }
}
