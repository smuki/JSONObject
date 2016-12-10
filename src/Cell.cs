using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Volte.Utils;

namespace Volte.Data.Json
{
    [Serializable]
        internal sealed class Cell {
            // Methods
            public Cell()
            {
            }

            public Cell(object oValue)
            {
                _Value = oValue;
            }

            public Cell(JSONObject oValue)
            {
                _Value = oValue["v"];

                if (oValue.ContainsKey("c")){
                    _sCode = oValue.GetValue("c");
                }
            }


            // Properties
            public object Text  { get { return _Value; } set { _Value = value; }  }
            public string sCode { get { return _sCode; } set { _sCode = value; }  }

            private string _sCode = string.Empty;
            private object _Value = new object();
        }
}
