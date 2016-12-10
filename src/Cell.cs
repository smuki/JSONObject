using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Volte.Utils;

namespace Volte.Data.Json
{
    public struct Cell
    {
        public string sCode;
        public object Value;
    }
}
