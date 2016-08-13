using System;
using System.Xml;
using System.IO;
using System.Text;
//using System.Web;
using System.Runtime.Serialization.Formatters.Binary;

namespace Volte.Data.JsonObject
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
                        JSONObjects _VContexts = new JSONObjects();
                        _VContexts.Read(_Lexer);

                        this.Name  = name;
                        this.Type  = "l";
                        this.Value = _VContexts;
                    } else {
                        this.Name  = name;
                        this.Value = _Lexer.ParseValue();
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
                    //ZZLogger.Debug(ZFILE_NAME , "type = "+this.Type);
                    if (this.Type == "v") {
                        writer.AppendLine();
                        ((JSONObject)this.Value).Write(writer);
                    } else if (this.Type == "l") {
                        writer.AppendLine();
                        ((JSONObjects)this.Value).Write(writer);
                    } else if (this.Type == "t") {
                        //  this.Value.Write(writer);
                    } else {
                        if (this.Type == "nvarchar") {
                            writer.Append("\"");
                            Util.EscapeString(writer, this.Value.ToString());
                            writer.Append("\"");
                        } else if (this.Type == "decimal" || this.Type == "integer") {
                            Util.EscapeString(writer, this.Value.ToString());
                        } else if (this.Type == "datetime") {
                            Util.EscapeString(writer, this.Value.ToString());
                        } else if (this.Type == "boolean") {
                            Util.EscapeString(writer, this.Value.ToString().ToLower());
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

        public string Type
        {
            get {
                return _type;
            } set {
                _type  = value;
            }
        }

        public string Name
        {
            get {
                return _name;
            } set {
                _name  = value;
            }
        }

        public object Value
        {
            get {
                return _value;
            } set {
                _value = value;
            }
        }

        private string _name  = "";
        private string _type  = "";
        private object _value = "";
    }
}
