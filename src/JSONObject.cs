using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Volte.Utils;

namespace Volte.Data.Json
{

    [Serializable]
        public class JSONObject {
            const string ZFILE_NAME = "JSONObject";

            // Methods
            public JSONObject()
            {
                _Dictionary = new Dictionary<string, JSONObjectPair> (StringComparer.InvariantCultureIgnoreCase);
            }

            public JSONObject(string cData)
            {
                _Dictionary = new Dictionary<string, JSONObjectPair> (StringComparer.InvariantCultureIgnoreCase);

                if (!string.IsNullOrEmpty(cData)) {
                    Parser(cData);
                }
            }

            public JSONObject(string Name,string Value)
            {

                _Dictionary = new Dictionary<string, JSONObjectPair> (StringComparer.InvariantCultureIgnoreCase);

                this.SetValue(Name,Value);

            }

            internal void Read(Lexer _Lexer)
            {
                _Lexer.SkipWhiteSpace();

                if (_Lexer.Current == '{' && _Lexer.NextChar != '}') {
                    _Lexer.NextToken();

                    for (;;) {
                        JSONObjectPair variable1 = new JSONObjectPair();

                        variable1.Read(_Lexer);

                        if (variable1.Value != null) {
                            this.SetValue(variable1.Name, variable1.Value, variable1.Type);
                        }

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
                writer.AppendLine("{");

                if (_Dictionary.Count > 0) {
                    int i = 0;

                    foreach (string name in _Dictionary.Keys) {
                        if (i > 0) {
                            writer.AppendLine(",");
                        }

                        _Dictionary[name].Write(writer);
                        i++;
                    }
                }

                writer.AppendLine("");
                writer.AppendLine("}");
            }

            public void Parser(string cString)
            {
                if (string.IsNullOrEmpty(cString)) {
                    return;
                }

                Lexer oLexer = new Lexer(cString);

                this.Read(oLexer);
            }

            public override string ToString()
            {

                s.Length = 0;
                this.Write(s);
                return s.ToString();
            }

            public bool ContainsKey(string name)
            {
                return _Dictionary.ContainsKey(name);
            }

            public void SetDateTime(string name, DateTime value)
            {
                this.SetValue(name, value, "datetime");
            }

            public void SetDateTime(string name, DateTime? value)
            {
                if (value.HasValue) {
                    this.SetValue(name , value.Value , "datetime");
                } else {
                    this.SetValue(name, "", "datetime");
                }
            }

            public void SetValue(string name, DateTime value)
            {
                this.SetValue(name, value, "datetime");
            }

            public DateTime GetDateTime(string Name)
            {
                object o = GetValue(Name);

                try {
                    return Util.ToDateTime(o);
                } catch {
                    return Util.DateTime_MinValue;
                }
            }

            public bool GetBoolean(string Name)
            {
                object o = GetValue(Name);

                try {
                    return Convert.ToBoolean(o);
                } catch {
                    return false;
                }
            }

            public int GetInteger(string Name)
            {
                object o = GetValue(Name);
                return Util.ToInt32(o);
            }

            public long GetLong(string Name)
            {
                object o = GetValue(Name);
                return Util.ToLong(o);
            }

            public decimal GetDecimal(string Name)
            {
                return Util.ToDecimal(GetValue(Name));
            }

            public void SetDouble(string name, double value)
            {
                this.SetValue(name, value, "");
            }

            public double GetDouble(string name)
            {
                return Convert.ToDouble(this.GetValue(name));
            }

            public void SetDecimal(string name, decimal value)
            {
                this.SetValue(name, value, "");
            }

            public bool IsJSONObject(string name)
            {
                return this.GetType(name) == "v";
            }

            public bool IsJSONArray(string name)
            {
                return this.GetType(name) == "l";
            }

            public JSONObject GetJSONObject(string Name)
            {
                JSONObject _JSONObject = new JSONObject();

                if (this.GetType(Name) == "v") {
                    if (_Dictionary.ContainsKey(Name)) {
                        _JSONObject = (JSONObject) _Dictionary[Name].Value;
                    }
                }

                return _JSONObject;
            }

            public JSONArray GetJSONArray(string Name)
            {
                JSONArray _JSONArray = new JSONArray();

                if (this.GetType(Name) == "l") {
                    if (_Dictionary.ContainsKey(Name)) {
                        _JSONArray = (JSONArray)_Dictionary[Name].Value;
                    }
                }

                return _JSONArray;
            }

            public string GetValue(string name)
            {
                if (_Dictionary.ContainsKey(name)) {
                    JSONObjectPair variable1 = _Dictionary[name];
                    if (variable1.Value==null){
                        return "";
                    }else{
                        return variable1.Value.ToString();
                    }
                } else {
                    return "";
                }
            }

            public string GetType(string name)
            {
                if (_Dictionary.ContainsKey(name)) {
                    JSONObjectPair variable1 = _Dictionary[name];
                    return variable1.Type;
                } else {
                    return "nvarchar";
                }
            }

            public void SetValue(string name, JSONArray value)
            {
                JSONObjectPair variable1 = new JSONObjectPair();

                variable1.Name    = name;
                variable1.Value   = value;
                variable1.Type    = "l";
                _Dictionary[name] = variable1;
            }

            public void SetValue(string name, JSONObject value)
            {
                JSONObjectPair variable1 = new JSONObjectPair();

                variable1.Name    = name;
                variable1.Value   = value;
                variable1.Type    = "v";
                _Dictionary[name] = variable1;
            }

            public void SetValue(string name, bool value)
            {
                this.SetBoolean(name, value);
            }

            public void SetValue(string name, int value)
            {
                this.SetInteger(name, value);
            }

            public void SetValue(string name, long value)
            {
                this.SetLong(name, value);
            }

            public void SetBoolean(string name, bool value)
            {
                JSONObjectPair variable1 = new JSONObjectPair();

                variable1.Name    = name;
                variable1.Value   = value;
                variable1.Type    = "boolean";
                _Dictionary[name] = variable1;
            }

            public void SetLong(string Name, long value)
            {
                JSONObjectPair variable1 = new JSONObjectPair();

                variable1.Name    = Name;
                variable1.Value   = value;
                variable1.Type    = "integer";
                _Dictionary[Name] = variable1;
            }

            public void SetInteger(string Name, int value)
            {
                JSONObjectPair variable1 = new JSONObjectPair();

                variable1.Name    = Name;
                variable1.Value   = value;
                variable1.Type    = "integer";
                _Dictionary[Name] = variable1;
            }

            public void SetValue(string name, string value)
            {
                this.SetValue(name, value, "");
            }

            public void Add(object key, object value)
            {
                SetValue(key.ToString(), value, "nvarchar");
            }

            public void Remove(object key)
            {
                _Dictionary.Remove(key.ToString());
            }

            public void SetValue(string name, object value, string cType)
            {
                if (_Dictionary == null) {
                    _Dictionary = new Dictionary<string, JSONObjectPair> (StringComparer.InvariantCultureIgnoreCase);
                }

                JSONObjectPair variable1 = new JSONObjectPair();

                variable1.Name    = name;
                variable1.Value   = value;
                variable1.Type    = cType;
                _Dictionary[name] = variable1;
            }

            public void Clear()
            {
                _Dictionary = new Dictionary<string, JSONObjectPair> (StringComparer.InvariantCultureIgnoreCase);
            }

            public List<string> Names
            {
                get {
                    List<string> _Names = new List<string>();

                    foreach (string name in _Dictionary.Keys) {
                        _Names.Add(name);
                    }

                    return _Names;
                }
            }

            public object this[object name]
            {
                get {
                    return this[name.ToString()];
                } set {
                }
            }

            public object this[string name]
            {
                get {
                    if (_Dictionary.ContainsKey(name)) {
                        return _Dictionary[name].Value;
                    } else {
                        return null;
                    }
                } set {
                    JSONObjectPair variable1 = new JSONObjectPair();

                    variable1.Name  = name;
                    variable1.Value = value;
                    if (value is DateTime) {
                        variable1.Type  = "datetime";
                    }else if (value is decimal) {
                        variable1.Type  = "decimal";
                    }else{
                        variable1.Type  = "nvarchar";
                    }

                    _Dictionary[name] = variable1;
                }
            }

            public int Count
            {
                get {
                    return _Dictionary.Count;
                }
            }

            private readonly StringBuilder s = new StringBuilder();

            private Dictionary<string, JSONObjectPair> _Dictionary = new Dictionary<string, JSONObjectPair> (StringComparer.InvariantCultureIgnoreCase);

        }
}
