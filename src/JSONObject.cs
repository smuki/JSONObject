using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Volte.Utils;

namespace Volte.Data.Json
{

    [Serializable]
    public class JSONObject
    {
        const string ZFILE_NAME = "JSONObject";

        // Methods
        public JSONObject()
        {
            _Dictionary = new Dictionary<string, JSONObjectPair>(StringComparer.InvariantCultureIgnoreCase);
        }

        public JSONObject(string cData)
        {
            _Dictionary = new Dictionary<string, JSONObjectPair>(StringComparer.InvariantCultureIgnoreCase);

            if (!string.IsNullOrEmpty(cData))
            {
                Parser(cData);
            }
        }

        public JSONObject(string Name, string Value)
        {

            _Dictionary = new Dictionary<string, JSONObjectPair>(StringComparer.InvariantCultureIgnoreCase);

            this.SetValue(Name, Value);

        }

        internal JSONObject(Lexer _Lexer)
        {

            _Dictionary = new Dictionary<string, JSONObjectPair>(StringComparer.InvariantCultureIgnoreCase);

            this.Read(_Lexer);
        }

        internal void Read(Lexer _Lexer)
        {

            if (_Lexer.MatchChar('{'))
            {
                _Lexer.NextToken();
                if (_Lexer.MatchChar('}'))
                {
                    _Lexer.NextToken();
                    return;
                }

                for (; ; )
                {
                    JSONObjectPair variable1 = new JSONObjectPair(_Lexer);

                    _Dictionary[variable1.Name] = variable1;

                    if (_Lexer.MatchChar(','))
                    {
                        _Lexer.NextToken();
                    }
                    else
                    {
                        break;
                    }
                }

                _Lexer.NextToken();
            }
        }

        internal void Write(StringBuilder writer)
        {
            writer.AppendLine("{");

            if (_Dictionary.Count > 0)
            {
                int i = 0;

                foreach (string name in _Dictionary.Keys)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (i > 0)
                        {
                            writer.AppendLine(",");
                        }

                        _Dictionary[name].Write(writer);
                        i++;
                    }
                }
            }

            writer.AppendLine("");
            writer.AppendLine("}");
        }

        public void Parser(string cString)
        {
            if (string.IsNullOrEmpty(cString))
            {
                return;
            }

            this.Read(new Lexer(cString));
        }

        public void Merge(JSONObject obj)
        {
            foreach (string name in obj.Names)
            {
                if (obj.IsJSONObject(name))
                {
                    this.SetValue(name, obj.GetJSONObject(name));
                }
                else if (obj.IsJSONArray(name))
                {
                    this.SetValue(name, obj.GetJSONArray(name));
                }
                else
                {
                    this.SetValue(name, obj.GetValue(name));
                }
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
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
            if (value.HasValue)
            {
                this.SetValue(name, value.Value, "datetime");
            }
            else
            {
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

            try
            {
                return Util.ToDateTime(o);
            }
            catch
            {
                return Util.DateTime_MinValue;
            }
        }

        public bool GetBoolean(string Name, bool bDefault = false)
        {
            object o = GetValue(Name);

            try
            {
                return Convert.ToBoolean(o);
            }
            catch
            {
                return bDefault;
            }
        }

        public int GetInteger(string Name, int nDefault = 0)
        {
            object o = GetValue(Name);
            if (o == null || string.IsNullOrEmpty(o.ToString()))
            {
                return nDefault;
            }
            return Util.ToInt32(o);
        }

        public long GetLong(string Name, int nDefault = 0)
        {
            object o = GetValue(Name);
            if (o == null || string.IsNullOrEmpty(o.ToString()))
            {
                return nDefault;
            }
            return Util.ToLong(o);
        }

        public decimal GetDecimal(string Name,decimal nDefault=0M)
        {
            object o = GetValue(Name);

            if (o == null || string.IsNullOrEmpty(o.ToString()))
            {
                return nDefault;
            }
            return Util.ToDecimal(o);
        }

        public void SetDouble(string name, double value)
        {
            this.SetValue(name, value, "decimal");
        }

        public double GetDouble(string name,double nDefault)
        {
            object o = GetValue(name);

            if (o == null || string.IsNullOrEmpty(o.ToString()))
            {
                return nDefault;
            }
            return Convert.ToDouble(o);
        }

        public void SetDecimal(string name, decimal value)
        {
            this.SetValue(name, value, "decimal");
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

            if (this.GetType(Name) == "v")
            {
                if (_Dictionary.ContainsKey(Name))
                {
                    _JSONObject = (JSONObject)_Dictionary[Name].Value;
                }
            }

            return _JSONObject;
        }

        public JSONArray GetJSONArray(string Name)
        {
            JSONArray _JSONArray = new JSONArray();

            if (this.GetType(Name) == "l")
            {
                if (_Dictionary.ContainsKey(Name))
                {
                    _JSONArray = (JSONArray)_Dictionary[Name].Value;
                }
            }

            return _JSONArray;
        }

        public string Concat(string names, string split)
        {
            string[] aValue = names.Split((new char[1] { ',' }));
            string sReturn = "";
            int i = 1;

            foreach (string sName in aValue)
            {
                if (sName != "")
                {
                    string s = this.GetValue(sName);
                    if (s != "")
                    {

                        sReturn = sReturn + s;
                        if (i < aValue.Length)
                        {
                            sReturn = sReturn + split;
                        }
                    }
                }
                i++;
            }
            return sReturn;
        }

        public bool HasAttr(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            int p = name.IndexOf(".");
            if (p > 0)
            {
                string f = name.Substring(0, p);
                string n = name.Substring(p + 1);
                return GetJSONObject(f).HasAttr(n);
            }
            else
            {
                return ContainsKey(name);
            }
        }

        public string Attr(string name , string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            int p = name.IndexOf(".");
            if (p > 0)
            {
                string f = name.Substring(0, p);
                string n = name.Substring(p + 1);
                return GetJSONObject(f).Attr(n , value);
            }
            else
            {
                return SetValue(name , value);
            }
        }

        public string Attr(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            int p = name.IndexOf(".");
            if (p > 0)
            {
                string f = name.Substring(0, p);
                string n = name.Substring(p + 1);
                return GetJSONObject(f).Attr(n);
            }
            else
            {
                return GetValue(name);
            }
        }

        public string GetValue(string name, string sDefault = "")
        {
            JSONObjectPair result = null;
            if (_Dictionary.TryGetValue(name, out result))
            {
                if (result.Value == null)
                {
                    return sDefault;
                }
                else
                {
                    return result.Value.ToString();
                }
            }
            return sDefault;
        }

        public string GetType(string name)
        {
            if (_Dictionary.ContainsKey(name))
            {
                JSONObjectPair variable1 = _Dictionary[name];
                return variable1.Type;
            }
            else
            {
                return "string";
            }
        }

        public void SetValue(string name, JSONArray value)
        {
            _Dictionary[name] = new JSONObjectPair(name, value, "l");
        }

        public void SetValue(string name, JSONObject value)
        {
            _Dictionary[name] = new JSONObjectPair(name, value, "v");
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
            _Dictionary[name] = new JSONObjectPair(name, value, "boolean");
        }
        public void SetLong(string name, long value)
        {
            _Dictionary[name] = new JSONObjectPair(name, value, "integer");
        }

        public void SetInteger(string name, int value)
        {
            _Dictionary[name] = new JSONObjectPair(name, value, "integer");
        }

        public void SetValue(string name, string value)
        {
            this.SetValue(name, value, "");
        }

        public void Add(object key, object value)
        {
            SetValue(key.ToString(), value, "string");
        }

        public void Remove(object key)
        {
            _Dictionary.Remove(key.ToString());
        }

        public void SetValue(string name, object value)
        {
            _Dictionary[name] = new JSONObjectPair(name, value);
        }

        public void SetValue(string name, object value, string cType)
        {
            _Dictionary[name] = new JSONObjectPair(name, value, cType);
        }

        public void Clear()
        {
            _Dictionary = new Dictionary<string, JSONObjectPair>(StringComparer.InvariantCultureIgnoreCase);
        }

        public JSONObject Clone()
        {
            StringBuilder s = new StringBuilder();
            this.Write(s);
            return new JSONObject(s.ToString());
        }

        public List<string> Names
        {
            get
            {
                List<string> _Names = new List<string>();

                foreach (string name in _Dictionary.Keys)
                {
                    _Names.Add(name);
                }

                return _Names;
            }
        }

        public object this[object name]
        {
            get
            {
                return this[name.ToString()];
            }
            set
            {
            }
        }

        public object this[string name]
        {
            get
            {
                JSONObjectPair result = null;
                if (_Dictionary.TryGetValue(name, out result))
                {
                    return result.Value;
                }
                return result;
            }
            set
            {
                string t = "string";
                if (value is DateTime)
                {
                    t = "datetime";
                }
                else if (value is decimal)
                {
                    t = "decimal";
                }
                else if (value is int)
                {
                    t = "integer";
                }

                _Dictionary[name] = new JSONObjectPair(name, value, t);
            }
        }

        public int Count
        {
            get
            {
                return _Dictionary.Count;
            }
        }

        private Dictionary<string, JSONObjectPair> _Dictionary = new Dictionary<string, JSONObjectPair>(StringComparer.InvariantCultureIgnoreCase);

    }
}
