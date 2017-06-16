using System;
using System.Text;
using System.Collections.Generic;

using Volte.Utils;

namespace Volte.Data.Json
{

    [Serializable]
        public class Row {
            const string ZFILE_NAME = "Row";
            // Methods
            public Row()
            {
                _cells     = new List<Cell>();
                _Reference = new JSONObject();
                _Reference.SetBoolean("a" , false);
            }

            public Row(int i)
            {
                _cells     = new List<Cell> (i);
                _Reference = new JSONObject();
                _Reference.SetBoolean("a" , false);
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
                                Cell _cell;

                                if (_JSONObject.ContainsKey("c")){
                                    _cell.sCode = _JSONObject.GetValue("c");
                                }else{
                                    _cell.sCode="";
                                }
                                _cell.Value = _JSONObject.GetValue("v");

                                _cells.Add(_cell);

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
                        _Reference.SetBoolean("a" , false);
                        _Reference.Read(_Lexer);

                        _Lexer.SkipWhiteSpace();
                    }

                    _Lexer.NextToken();

                }
            }

            private void Write(StringBuilder  writer , string sName , object o)
            {


                if (_Flatten==Flatten.Value){

                }else if (_Flatten==Flatten.NameValue){
                    writer.Append("\"" + sName + "\":");
                }else{
                    writer.Append("\"" + sName + "\":");
                }

                if (o is DateTime) {
                    if ((DateTime)o<= Util.DateTime_MinValue) {
                        writer.Append("\"");
                        writer.Append("\"");
                    } else {
                        writer.Append("\"");
                        Util.EscapeString(writer ,((DateTime)o).ToString("yyyyMMddhhmmss"));
                        writer.Append("\"");
                    }
                }else if (o is decimal || o is int) {

                    Util.EscapeString(writer, o.ToString());

                } else if (o is bool) {

                    Util.EscapeString(writer, o.ToString().ToLower());

                } else {

                    writer.Append("\"");
                    if (o!=null){
                        Util.EscapeString(writer, o.ToString());
                    }
                    writer.Append("\"");
                }
            }

            internal void Write(StringBuilder writer , Columns _columns)
            {
                if (_Flatten==Flatten.Value){

                    writer.AppendLine("[");
                    if (_cells != null) {
                        bool first = true;

                        foreach (Cell _Cell in _cells) {
                            if (!first) {
                                writer.AppendLine(",");
                            } else {
                                first = false;
                            }
                            this.Write(writer , "v" , _Cell.Value);
                        }
                    }
                    writer.Append("]");

                }else if (_Flatten==Flatten.NameValue){

                    writer.AppendLine("{");
                    if (_cells != null) {
                        bool first = true;

                        int i=0;
                        foreach (Cell _Cell in _cells) {
                            if (!first) {
                                writer.AppendLine(",");
                            } else {
                                first = false;
                            }
                            this.Write(writer , _columns.Fields[i].Name , _Cell.Value);
                            i++;
                        }
                    }
                    writer.Append("}");
                }else{

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

                            writer.Append("{");
                            this.Write(writer , "v" , _Cell.Value);

                            if (_Cell.sCode!=string.Empty){
                                writer.Append(",");

                                this.Write(writer , "c" , _Cell.sCode);
                            }
                            writer.Append("}");

                        }

                        writer.Append("]");
                        writer.AppendLine(",");
                        writer.AppendLine("");
                    }
                    writer.Append("\"Reference\":");

                    if (_Reference==null){

                        _Reference = new JSONObject();
                        _Reference.SetBoolean("a" , false);
                    }
                    _Reference.Write(writer);

                    writer.AppendLine("");
                    writer.AppendLine("}");
                }
            }

            public decimal GetDecimal(int i)
            {
                return Util.ToDecimal(this[i].Value);
            }

            public DateTime GetDateTime(int i)
            {
                return Util.ToDateTime(this[i].Value);
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

            public int    Index         { get { return _Index;      } set { _Index      = value; }  }
            public JSONObject Reference { get { return _Reference;  } set { _Reference  = value; }  }
            public Flatten Flatten      { get { return _Flatten;    } set { _Flatten    = value; }  }

            // Fields
            private int _Index       = -1;
            private int _size        = 0;
            private Flatten _Flatten = Flatten.Complex;

            private List<Cell> _cells     = new List<Cell>();
            private JSONObject _Reference = new JSONObject();
        }
}
