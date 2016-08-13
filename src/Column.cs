using System;
using System.Text;

namespace Volte.Data.JsonObject
{

    [Serializable]
        public class Column: AttributeMapping {
            const string ZFILE_NAME = "Column";
            private JSONObject _NameValue;
            // Methods
            public Column()
            {
                this.Index = 0;
            }

            internal void Read(Lexer _Lexer)
            {
                if (_Lexer.Current == '{') {
                    _Lexer.NextToken();

                    string name = _Lexer.ParseName();

                    if (name == "define") {

                        _NameValue = new JSONObject();
                        _NameValue.Read(_Lexer);

                        if (_NameValue.ContainsKey("DataType")) {

                            this.DataType = _NameValue.GetValue("DataType");

                        } else if (_NameValue.ContainsKey("Name")) {

                            this.Name = _NameValue.GetValue("Name");

                        } else if (_NameValue.ContainsKey("Caption")) {

                            this.Caption =  _NameValue.GetValue("Caption");

                        } else if (_NameValue.ContainsKey("ColumnName")) {

                            this.ColumnName =  _NameValue.GetValue("ColumnName");

                        } else if (_NameValue.ContainsKey("Index")) {

                            this.Index =  _NameValue.GetInteger("Index");

                        } else if (_NameValue.ContainsKey("Width")) {

                            this.Width =  _NameValue.GetInteger("Width");

                        } else if (_NameValue.ContainsKey("Scale")) {

                            this.Scale =  _NameValue.GetInteger("Scale");

                        } else if (_NameValue.ContainsKey("Reference")) {

                            this.Reference =  _NameValue.GetValue("Reference");

                        } else if (_NameValue.ContainsKey("EnableMode")) {

                            this.EnableMode =  _NameValue.GetValue("EnableMode");

                        } else if (_NameValue.ContainsKey("AlignName")) {

                            this.AlignName =  _NameValue.GetValue("AlignName");

                        } else if (_NameValue.ContainsKey("NonPrintable")) {

                            this.NonPrintable =  _NameValue.GetBoolean("NonPrintable");

                        }

                    }

                    _Lexer.NextToken();

                }
            }

            internal void Write(StringBuilder writer)
            {
                _NameValue = new JSONObject();

                _NameValue.SetValue("DataType"       , this.DataType     );
                _NameValue.SetValue("Name"           , this.Name         );
                _NameValue.SetValue("Caption"        , this.Caption      );
                _NameValue.SetValue("ColumnName"     , this.ColumnName   );
                _NameValue.SetInteger("Index"        , this.Index        );
                _NameValue.SetInteger("Width"        , this.Width        );
                _NameValue.SetInteger("Scale"        , this.Scale        );
                _NameValue.SetValue("Reference"      , this.Reference    );
                _NameValue.SetValue("EnableMode"     , this.EnableMode   );
                _NameValue.SetValue("AlignName"      , this.AlignName    );
                _NameValue.SetBoolean("NonPrintable" , this.NonPrintable );


                writer.AppendLine("{");
                writer.AppendLine("\"define\":");

                _NameValue.Write(writer);

                writer.AppendLine("}");
            }
        }
}
