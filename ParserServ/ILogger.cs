using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserServ
{
    internal interface ILogger
    {
        public void Load();
        public void Save();
    }
}
