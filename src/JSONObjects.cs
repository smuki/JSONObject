using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Volte.Data.Json
{

    [Serializable]
        public class JSONObjects {
            const string ZFILE_NAME = "JSONObjects";

            // Methods
            public JSONObjects()
            {
                _Dictionary = new List<JSONObject>();
            }

            public JSONObjects(string cData)
            {
                _Dictionary  = new List<JSONObject>();
                _Dictionary2 = new List<JSONObjects>();

                if (!string.IsNullOrEmpty(cData)) {
                    Parser(cData);
                }
            }

            internal void Read(Lexer _Lexer)
            {
                _Lexer.SkipWhiteSpace();

                if (_Lexer.Current == '[' && _Lexer.NextChar != ']') {
                    _Lexer.NextToken();

                    for (;;) {
                        if (_Lexer.Current == '[') {
                            JSONObjects variable2 = new JSONObjects();
                            variable2.Read(_Lexer);
                            this.Add(variable2);
                        } else {
                            JSONObject variable1 = new JSONObject();

                            variable1.Read(_Lexer);

                            this.Add(variable1);
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

                if (_Dictionary.Count > 0) {

                    foreach (JSONObject name in _Dictionary) {
                        if (i > 0) {
                            writer.Append(",");

                            writer.AppendLine("");
                        }

                        name.Write(writer);
                        i++;
                    }
                }

                if (_Dictionary2.Count > 0) {
                    foreach (JSONObjects name in _Dictionary2) {
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

            private string _ToString()
            {

                s.Length = 0;

                this.Write(s);
                return s.ToString();
            }

            public void Add(JSONObjects value)
            {
                _Dictionary2.Add(value);
            }
            public void Remove(JSONObjects value)
            {
                _Dictionary2.Remove(value);
            }
            public void Add(JSONObject value)
            {
                _Dictionary.Add(value);
            }

            public void Remove(JSONObject value)
            {
                _Dictionary.Remove(value);
            }

            public void Clear()
            {
                _Dictionary  = new List<JSONObject>();
                _Dictionary2 = new List<JSONObjects>();
            }

            public List<JSONObjects> ListValues
            {
                get {
                    return _Dictionary2;
                }
            }

            public List<JSONObject> Values
            {
                get {
                    return _Dictionary;
                }
            }

            public int Count
            {
                get {
                    return _Dictionary.Count + _Dictionary2.Count;
                }
            }

            // JSONObjects
            private readonly StringBuilder s = new StringBuilder();

            private List<JSONObject> _Dictionary = new List<JSONObject>();
            private List<JSONObjects> _Dictionary2 = new List<JSONObjects>();

        }
}
