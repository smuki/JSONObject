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

            internal JSONObjectPair(Lexer _Lexer)
            {
                this.Read(_Lexer);
            }

            internal void Read(Lexer _Lexer)
            {

                if (_Lexer.MatchChar('{')) {
                    this.Value = new JSONObject(_Lexer);
                } else {
                    if (!_Lexer.MatchChar('}')) {
                        string name = _Lexer.ParseName();
                        _Lexer.SkipWhiteSpace();
                        char ch   = _Lexer.Current;

                        this.Name = name;

                        if (ch=='{') {

                            this.Type  = "v";
                            this.Value = new JSONObject(_Lexer);

                        } else if (ch=='[') {
                            this.Type  = "l";
                            this.Value =new JSONArray(_Lexer);
                        } else {

                            if (char.IsDigit(ch) || ch== '-') {
                                this.Type  = "decimal";
                                this.Value = _Lexer.ParseValue();
                            }else if (ch== 'f' || ch== 't') {
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
                        } else {
                            if (this.Type == "decimal" || this.Type == "integer" || this.Value is decimal || this.Value is int ) {
                                writer.Append(Util.ToDecimal(this.Value).ToString());
                            } else if (this.Type == "boolean" || this.Value is bool ) {
                                writer.Append(Util.ToBoolean(this.Value).ToString().ToLower());
                            } else if (this.Type == "datetime" || this.Value is DateTime) {

                                if (this.Value==null || string.IsNullOrEmpty(this.Value.ToString())) {
                                    writer.Append("\"\"");
                                }else if (this.Value is DateTime) {
                                    if ((DateTime)this.Value <= Util.DateTime_MinValue) {
                                        writer.Append("\"\"");
                                    } else {
                                        writer.Append("\"");
                                        writer.Append(Util.DateTimeToMilliSecond((DateTime)this.Value));
                                        writer.Append("\"");
                                    }
                                }else{
                                    writer.Append("\"");
                                    writer.Append(Util.DateTimeToMilliSecond(Util.ToDateTime(this.Value)));
                                    writer.Append("\"");
                                }

                            } else {
                                writer.Append("\"");
                                Util.EscapeString(writer, this.Value.ToString());
                                writer.Append("\"");
                            }
                        }
                    } else {
                        writer.Append("null");
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
