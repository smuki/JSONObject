using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Volte.Data.Json
{

    [Serializable]
        public class JSONArray {
            const string ZFILE_NAME = "JSONArray";

            // Methods
            public JSONArray()
            {
                _JSONObjects = new List<JSONObject>();
                _JSONArrays  = new List<JSONArray>();
            }

            public JSONArray(string cData)
            {
                _JSONObjects = new List<JSONObject>();
                _JSONArrays  = new List<JSONArray>();

                if (!string.IsNullOrEmpty(cData)) {
                    Parser(cData);
                }
            }
            internal JSONArray(Lexer _Lexer)
            {
                _JSONObjects = new List<JSONObject>();
                _JSONArrays  = new List<JSONArray>();

                this.Read(_Lexer);
            }

            internal void Read(Lexer _Lexer)
            {
                _Lexer.SkipWhiteSpace();

                if (_Lexer.Current == '[') {
                    _Lexer.NextToken();
                    if (_Lexer.Current == ']')
                    {
                        _Lexer.NextToken();
                        return;
                    }

                    for (;;) {
                        if (_Lexer.Current == '[') {
                            JSONArray variable2 = new JSONArray();
                                variable2.Read(_Lexer);
                                this.Add(variable2);
                        } else if (_Lexer.Current == '{') {
                            JSONObject variable1 = new JSONObject();

                            variable1.Read(_Lexer);

                            this.Add(variable1);
                        }else{

                            string _s="";
                            if (char.IsDigit(_Lexer.Current) || _Lexer.Current == '-') {
                                _s= _Lexer.ParseValue();
                            }else if (_Lexer.Current == 'f' || _Lexer.Current == 't') {
                                _s= _Lexer.ParseValue();
                            }else{
                                _s= _Lexer.ParseValue();
                            }

                            this.Add(_s);

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
                writer.AppendLine("[");

                int i = 0;

                if (_Object.Count > 0) {
                    foreach (object sValue in _Object) {
                        if (i > 0) {
                            writer.Append(",");

                            writer.AppendLine("");
                        }

                        if (sValue is decimal || sValue is int || sValue is double || sValue is float){
                            writer.Append(sValue.ToString());
                        }else{
                            writer.Append("\"");
                            writer.Append(sValue.ToString());
                            writer.AppendLine("\"");
                        }
                        i++;
                    }
                }

                if (_JSONObjects.Count > 0) {

                    foreach (JSONObject name in _JSONObjects) {
                        if (i > 0) {
                            writer.Append(",");

                            writer.AppendLine("");
                        }

                        name.Write(writer);
                        i++;
                    }
                }

                if (_JSONArrays.Count > 0) {
                    foreach (JSONArray name in _JSONArrays) {
                        if (i > 0) {
                            writer.Append(",");

                            writer.AppendLine("");
                        }

                        name.Write(writer);
                        i++;
                    }
                }

                writer.AppendLine("");
                writer.Append("]");
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
                return _ToString();
            }

            public JSONObject JSONObject(int index)
            {

                if (index<_JSONObjects.Count){
                    return _JSONObjects[index];
                }else{
                    return new JSONObject();
                }

            }

            public JSONArray Select(string name , string value)
            {
                JSONArray _JSONArray = new JSONArray();

                foreach (JSONObject _record in _JSONObjects) {
                    if (_record.GetValue(name)==value){
                       _JSONArray.Add(_record);
                    }
                }
                return _JSONArray;
            }
            public JSONObject Lookup(string name , string value)
            {
                foreach (JSONObject _record in _JSONObjects) {
                    if (_record.GetValue(name)==value){
                        return _record;
                    }
                }
                return new JSONObject();
            }
            private string _ToString()
            {

                s.Length = 0;

                this.Write(s);
                return s.ToString();
            }

            public void Add(JSONArray value)
            {
                _JSONArrays.Add(value);
            }
            public void Remove(JSONArray value)
            {
                _JSONArrays.Remove(value);
            }
            public void Add(JSONObject value)
            {
                _JSONObjects.Add(value);
            }

            public void Add(object value)
            {
                _Object.Add(value);
            }

            public void Remove(JSONObject value)
            {
                _JSONObjects.Remove(value);
            }

            public void Remove(object value)
            {
                _Object.Remove(value);
            }

            public void Clear()
            {
                _JSONObjects  = new List<JSONObject>();
                _JSONArrays = new List<JSONArray>();
            }

            public List<JSONArray> JSONArrays
            {
                get {
                    return _JSONArrays;
                }
            }

            public List<object> Names
            {
                get {
                    return _Object;
                }
            }

            public List<JSONObject> JSONObjects
            {
                get {
                    return _JSONObjects;
                }
            }

            public int Count
            {
                get {
                    return _JSONObjects.Count + _JSONArrays.Count + _Object.Count;
                }
            }

            // JSONArray
            private readonly StringBuilder s = new StringBuilder();

            private List<JSONObject> _JSONObjects = new List<JSONObject>();
            private List<JSONArray>  _JSONArrays  = new List<JSONArray>();
            private List<object>     _Object      = new List<object>();

        }
}
