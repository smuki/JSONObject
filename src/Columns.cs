using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Volte.Data.Json
{

    [Serializable]
    internal class Columns {

        // Methods
        internal Columns()
        {
        }

        public void Add(Column column)
        {
            if (column == null) {
                throw new ArgumentNullException("column");
            }

            _Data.Add(column);
        }

        internal void Read(Lexer _Lexer)
        {
            _Lexer.SkipWhiteSpace();

            if (_Lexer.Current == '[') {
                _Lexer.NextToken();

                for (;;) {
                    Column column1 = new Column();

                    column1.Read(_Lexer);
                    _Data.Add(column1);
                    _Lexer.SkipWhiteSpace();

                    if (_Lexer.Current == ',') {
                        _Lexer.NextToken();
                    } else {
                        break;
                    }
                }

                _Lexer.NextToken();
            }
        }

        internal void Write(StringBuilder writer)
        {
            writer.AppendLine("\"columns\":");
            writer.AppendLine("[");

            for (int num1 = 0; num1 < _Data.Count; num1++) {
                if (num1 > 0) {
                    writer.AppendLine(",");
                }

                _Data[num1].Write(writer);
            }

            writer.AppendLine("]");

        }

        public bool ContainsKey(string name)
        {
            if (FieldDict == null) {
                FieldDict = new Dictionary<string, int>();

                for (int j = 0; j < _Data.Count; j++) {
                    FieldDict[_Data[j].Name.ToLower()] = j;
                }
            }

            return FieldDict.ContainsKey(name.ToLower());
        }

        public int Ordinal(string name)
        {
            if (string.IsNullOrEmpty(name)) {
                return -1;
            }

            name = name.Replace(".", "_");

            if (ContainsKey(name.ToLower())) {
                return FieldDict[name.ToLower()];
            } else {
                return -1;
            }
        }

        // Properties
        public int Count              { get { return  _Data.Count;  }  }
        public Column this[int index] { get { return  _Data[index]; }  }
        public List<Column> Fields    { get { return  _Data;        }  }

        // Columns
        private Dictionary<string, int> FieldDict;
        private List<Column> _Data = new List<Column>();
    }
}
