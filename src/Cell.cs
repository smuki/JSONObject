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
                _index = 0;
            }

            public Cell(object text)
            {
                _index    = 0;
                _Data     = new JSONObject();
                this.Text = text;
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
            public object Text
            {
                get {
                    return _Data["value"];
                } set {
                    _Data["value"] = value;
                }
            }

            // Fields

            public int Index
            {
                get {
                    return _index;
                } set {
                    _index = value;
                }
            }
            private JSONObject _Data = new JSONObject();
            private int   _index;
        }
}
