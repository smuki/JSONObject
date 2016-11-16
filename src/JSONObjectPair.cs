using System;
using System.Text;

using Volte.Utils;

namespace Volte.Data.Json
{

    [Serializable]
        internal class JSONObjectPair {
            // Methods
            const string ZFILE_NAME = "JSONObjectPair";
            public JSONObjectPair()
            {
            }

            internal void Read(Lexer _Lexer)
            {

                if (_Lexer.Current == '{') {
                    JSONObject _VContexts = new JSONObject();

                    _VContexts.Read(_Lexer);
                    this.Value = _VContexts;
                } else {
                    _Lexer.SkipWhiteSpace();

                    if (_Lexer.Current != '}') {
                        string name = _Lexer.ParseName();
                        _Lexer.SkipWhiteSpace();

                        if (_Lexer.Current == '{') {
                            JSONObject _VContexts = new JSONObject();
                            _VContexts.Read(_Lexer);

                            this.Name  = name;
                            this.Type  = "v";
                            this.Value = _VContexts;

                        } else if (_Lexer.Current == '[') {
                            JSONArray _VContexts = new JSONArray();
                            _VContexts.Read(_Lexer);

                            this.Name  = name;
                            this.Type  = "l";
                            this.Value = _VContexts;
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
                            if (this.Type == "decimal" || this.Type == "integer") {
                                Util.EscapeString(writer, this.Value.ToString());
                            } else if (this.Type == "datetime") {
                                writer.Append("\"");

                                Util.EscapeString(writer ,((DateTime)this.Value).ToString("yyyyMMddhhmmss"));
                                writer.Append("\"");
                            } else if (this.Type == "boolean") {
                                Util.EscapeString(writer, this.Value.ToString().ToLower());
                            } else if (this.Value is DateTime) {
                                if ((DateTime)this.Value <= Util.DateTime_MinValue) {
                                    writer.Append("\"");
                                    writer.Append("\"");
                                } else {
                                    writer.Append("\"");
                                    Util.EscapeString(writer ,((DateTime)this.Value).ToString("yyyyMMddhhmmss"));
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
