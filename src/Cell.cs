using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
                _Data      = new JSONObject();
                _Data["v"] = oValue;
            }

            public Cell(JSONObject oValue)
            {
                _Data["v"] = oValue["v"];
            }
            internal void Read(Lexer element)
            {
                _Data.Read(element);
            }

            internal void Write(StringBuilder  writer)
            {
                _Data.Write(writer);
            }

            // Properties
            public object Text     { get { return _Data["v"]; } set { _Data["v"] = value; }  }
            public JSONObject Data { get { return _Data;      } set { _Data      = value; }  }

            private JSONObject _Data = new JSONObject();
        }
}
