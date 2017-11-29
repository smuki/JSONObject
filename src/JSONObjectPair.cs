using System;
using System.Text;

using Volte.Utils;

namespace Volte.Data.Json
{

    [Serializable]
        internal sealed class JSONObjectPair {
            // Methods
            const string ZFILE_NAME = "JSONObjectPair";
            public JSONObjectPair()
            {
            }

            public JSONObjectPair(string name , object value)
            {
                _name  = name;
                _value = value;
            }

            public JSONObjectPair(string name , object value , string type)
            {
                _type  = type;
                _name  = name;
                _value = value;
            }

            internal void Read(Lexer _Lexer)
            {

                if (_Lexer.Current == '{') {
                    JSONObject _obj = new JSONObject();

                    _obj.Read(_Lexer);
                    this.Value = _obj;
                } else {
                    if (!_Lexer.MatchChar('}')) {
                        string name = _Lexer.ParseName();
                        if (_Lexer.MatchChar('{')) {
                            JSONObject _obj = new JSONObject();
                            _obj.Read(_Lexer);

                            this.Name  = name;
                            this.Type  = "v";
                            this.Value = _obj;

                        } else if (_Lexer.MatchChar('[')) {
                            JSONArray _obj = new JSONArray();
                            _obj.Read(_Lexer);

                            this.Name  = name;
                            this.Type  = "l";
                            this.Value = _obj;
                        } else {

                            this.Name  = name;
                            if (char.IsDigit(_Lexer.Current) || _Lexer.Current == '-') {
                                this.Type  = "decimal";
                                this.Value = _Lexer.ParseValue();
                            }else if (_Lexer.Current == 'f' || _Lexer.Current == 't') {
                                this.Value = _Lexer.ParseValue();
                                this.Type  = "boolean";
                            }else{
                                this.Value = _Lexer.ParseValue();
                            }
                        }

                    } else {
                        this.Value = null;
                    }
                }
            }

            internal void Write(StringBuilder  writer)
            {
                if (!string.IsNullOrEmpty(this.Name)) {
                    writer.Append("\"" + this.Name + "\":");

                    if (this.Value != null) {
                        if (this.Type == "v") {
                            writer.AppendLine();
                            ((JSONObject)this.Value).Write(writer);
                        } else if (this.Type == "l") {
                            writer.AppendLine();
                            ((JSONArray)this.Value).Write(writer);
                        } else if (this.Type == "t") {
                            //  this.Value.Write(writer);
                        } else {
                            if (this.Value is decimal || this.Value is int || this.Type == "decimal" || this.Type == "integer") {
                                Util.EscapeString(writer, this.Value.ToString());
                            } else if (this.Value is bool || this.Type == "boolean") {
                                Util.EscapeString(writer, this.Value.ToString().ToLower());
                            } else if (this.Type == "datetime" || this.Value is DateTime) {

                                if (this.Value is DateTime) {
                                    if ((DateTime)this.Value <= Util.DateTime_MinValue) {
                                        writer.Append("\"\"");
                                    } else {
                                        writer.Append("\"");
                                        Util.EscapeString(writer ,((DateTime)this.Value).ToString("yyyyMMddhhmmss"));
                                        writer.Append("\"");
                                    }
                                }else{
                                    writer.Append("\"");

                                    Util.EscapeString(writer ,Util.ToDateTime(this.Value).ToString("yyyyMMddhhmmss"));
                                    writer.Append("\"");
                                }

                            } else {
                                writer.Append("\"");
                                Util.EscapeString(writer, this.Value.ToString());
                                writer.Append("\"");
                            }
                        }
                    } else {
                        writer.Append("\"\"");
                    }
                }
            }

            public string Type  { get { return _type;  } set { _type  = value; }  }
            public string Name  { get { return _name;  } set { _name  = value; }  }
            public object Value { get { return _value; } set { _value = value; }  }

            private string _name  = "";
            private string _type  = "";
            private object _value = "";
        }
}
