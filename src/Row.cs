using System;
using System.Text;
using System.Collections.Generic;

using Volte.Utils;

namespace Volte.Data.Json
{

    [Serializable]
        internal class Row {
            const string ZFILE_NAME = "Row";
            // Methods
            public Row()
            {
                _cells     = new List<Cell>();
                _Reference = new JSONObject();
            }

            public Row(int i)
            {
                _cells     = new List<Cell> (i);
                _Reference = new JSONObject();
            }

            internal void Read(Lexer _Lexer)
            {
                if (_Lexer.Current == '{') {
                    _Lexer.NextToken();

                    string name = _Lexer.ParseName();

                    int c = 0;

                    if (name == "cells") {

                        _Lexer.SkipWhiteSpace();

                        if (_Lexer.Current == '[') {
                            _Lexer.NextToken();
                            _Lexer.SkipWhiteSpace();

                            for (;;) {
                                c++;

                                JSONObject _JSONObject = new JSONObject();
                                _JSONObject.Read(_Lexer);
                                _cells.Add(new Cell(_JSONObject));

                                if (_Lexer.Current == ',') {
                                    _Lexer.NextToken();
                                } else {
                                    break;
                                }
                            }
                        }

                        _Lexer.NextToken();
                    }

                    if (_Lexer.Current == ',') {
                        _Lexer.NextToken();
                    }

                    name = _Lexer.ParseName();

                    if (name == "Reference") {

                        _Reference = new JSONObject();
                        _Reference.Read(_Lexer);

                        _Lexer.SkipWhiteSpace();
                    }

                    _Lexer.NextToken();

                }
            }

            internal void Write(StringBuilder writer)
            {
                writer.AppendLine("{");

                if (_cells != null) {
                    writer.Append("\"cells\":");
                    writer.Append("[");
                    bool first = true;

                    foreach (Cell _Cell in _cells) {
                        if (!first) {
                            writer.AppendLine(",");
                        } else {
                            first = false;
                        }

                        _Cell.Write(writer);
                    }

                    writer.Append("]");
                    writer.AppendLine(",");
                    writer.AppendLine("");
                }
                writer.Append("\"Reference\":");

                if (_Reference==null){

                    _Reference = new JSONObject();
                }
                _Reference.Write(writer);

                writer.AppendLine("");
                writer.AppendLine("}");
            }

            public DateTime getDateTime(int i)
            {
                return Util.ToDateTime(this[i]);
            }

            public Cell this[int i]
            {
                get {
                    if (i >= _cells.Count) {
                        _size = _cells.Count;

                        for (int x = _size; x <= i; x++) {
                            _size++;
                            _cells.Add(new Cell());
                        }
                    }

                    return _cells[i];
                } set {
                    if (i >= _cells.Count) {
                        _size = _cells.Count;

                        for (int x = _size; x <= i; x++) {
                            _size++;

                            if (x >= i) {
                                _cells.Add(value);
                            } else {
                                _cells.Add(new Cell());
                            }
                        }
                    } else {
                        _cells[i] = value;
                    }
                }
            }

            public int    Index         { get { return _Index;     } set { _Index     = value; }  }
            public JSONObject Reference { get { return _Reference; } set { _Reference = value;  }  }

            // Fields
            private int _Index = -1;
            private int _size  = 0;

            private List<Cell> _cells     = new List<Cell>();
            private JSONObject _Reference = new JSONObject();
        }
}
