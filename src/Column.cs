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

                _NameValue = new JSONObject();
                _NameValue.Read(_Lexer);

                if (_NameValue.ContainsKey("DataType")) {

                    this.DataType = _NameValue.GetValue("DataType");

                }
                if (_NameValue.ContainsKey("Name")) {

                    this.Name = _NameValue.GetValue("Name");

                }
                if (_NameValue.ContainsKey("Caption")) {

                    this.Caption =  _NameValue.GetValue("Caption");

                }
                if (_NameValue.ContainsKey("ColumnName")) {

                    this.ColumnName =  _NameValue.GetValue("ColumnName");

                }
                if (_NameValue.ContainsKey("Index")) {

                    this.Index =  _NameValue.GetInteger("Index");

                }
                if (_NameValue.ContainsKey("Width")) {

                    this.Width =  _NameValue.GetInteger("Width");

                }
                if (_NameValue.ContainsKey("Scale")) {

                    this.Scale =  _NameValue.GetInteger("Scale");

                }
                if (_NameValue.ContainsKey("Reference")) {

                    this.Reference =  _NameValue.GetValue("Reference");

                }
                if (_NameValue.ContainsKey("EnableMode")) {

                    this.EnableMode =  _NameValue.GetValue("EnableMode");

                }
                if (_NameValue.ContainsKey("AlignName")) {

                    this.AlignName =  _NameValue.GetValue("AlignName");

                }
                if (_NameValue.ContainsKey("NonPrintable")) {

                    this.NonPrintable =  _NameValue.GetBoolean("NonPrintable");

                }


            }

            internal void Write(StringBuilder writer)
            {
                _NameValue = new JSONObject();

                _NameValue.SetValue("Name"     , this.Name);
                _NameValue.SetValue("Caption"  , this.Caption);
                _NameValue.SetValue("DataType" , this.DataType);
                if (this.EnableMode!=""){
                    _NameValue.SetValue("EnableMode" , this.EnableMode);
                }
                if (this.ColumnName!=""){
                    _NameValue.SetValue("ColumnName" , this.ColumnName);
                }
                _NameValue.SetInteger("Width"   , this.Width);
                _NameValue.SetInteger("Scale"   , this.Scale);
                if (this.Reference!=""){
                    _NameValue.SetValue("Reference" , this.Reference);
                }
                if (this.AlignName!=""){
                    _NameValue.SetValue("AlignName" , this.AlignName);
                }
                _NameValue.SetBoolean("NonPrintable" , this.NonPrintable);
                _NameValue.SetInteger("Index"        , this.Index);

                _NameValue.Write(writer);

            }
        }
}
