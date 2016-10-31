using System;
using System.Text;

namespace Volte.Data.Json
{

    [Serializable]
        public class Column: AttributeMapping {
            const string ZFILE_NAME = "Column";
            private JSONObject _Property;
            // Methods
            public Column()
            {
                this.Index = -1;
            }

            internal void Read(Lexer _Lexer)
            {

                _Property = new JSONObject();
                _Property.Read(_Lexer);

                if (_Property.ContainsKey("DataType")) {

                    this.DataType = _Property.GetValue("DataType");

                }
                if (_Property.ContainsKey("Name")) {

                    this.Name = _Property.GetValue("Name");

                }
                if (_Property.ContainsKey("Caption")) {

                    this.Caption =  _Property.GetValue("Caption");

                }
                if (_Property.ContainsKey("ColumnName")) {

                    this.ColumnName =  _Property.GetValue("ColumnName");

                }
                if (_Property.ContainsKey("Index")) {

                    this.Index =  _Property.GetInteger("Index");

                }
                if (_Property.ContainsKey("Width")) {

                    this.Width =  _Property.GetInteger("Width");

                }
                if (_Property.ContainsKey("Scale")) {

                    this.Scale =  _Property.GetInteger("Scale");

                }
                if (_Property.ContainsKey("Reference")) {

                    this.Reference =  _Property.GetValue("Reference");

                }
                if (_Property.ContainsKey("EnableMode")) {

                    this.EnableMode =  _Property.GetValue("EnableMode");

                }
                if (_Property.ContainsKey("AlignName")) {

                    this.AlignName =  _Property.GetValue("AlignName");

                }
                if (_Property.ContainsKey("NonPrintable")) {

                    this.NonPrintable =  _Property.GetBoolean("NonPrintable");

                }

            }

            internal void Write(StringBuilder writer)
            {
                _Property = new JSONObject();

                _Property.SetValue("Name"     , this.Name);
                _Property.SetValue("Caption"  , this.Caption);
                _Property.SetValue("DataType" , this.DataType);
                _Property.SetInteger("Width"  , this.Width);
                _Property.SetInteger("Scale"  , this.Scale);
                if (this.EnableMode!=""){
                    _Property.SetValue("EnableMode" , this.EnableMode);
                }
                if (this.ColumnName!=""){
                    _Property.SetValue("ColumnName" , this.ColumnName);
                }
                if (this.Reference!=""){
                    _Property.SetValue("Reference" , this.Reference);
                }
                if (this.AlignName!=""){
                    _Property.SetValue("AlignName" , this.AlignName);
                }
                _Property.SetValue("ClassName"      , this.ClassName);
                _Property.SetBoolean("NonPrintable" , this.NonPrintable);

                if (this.Index>=0){
                    _Property.SetInteger("Index"        , this.Index);
                }


                _Property.Write(writer);

            }
        }
}
