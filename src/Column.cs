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

                if (_Property.ContainsKey("dataType")) {

                    this.DataType = _Property.GetValue("dataType");

                }
                if (_Property.ContainsKey("name")) {

                    this.Name = _Property.GetValue("name");

                }
                if (!string.IsNullOrEmpty(this.Name)) {

                    this.Hash = this.Name.Replace("." , "_");

                }
                if (_Property.ContainsKey("caption")) {

                    this.Caption = _Property.GetValue("caption");

                }
                if (_Property.ContainsKey("description")) {

                    this.Description = _Property.GetValue("description");

                }
                if (_Property.ContainsKey("columnName")) {

                    this.ColumnName = _Property.GetValue("columnName");

                }
                if (_Property.ContainsKey("index")) {

                    this.Index =  _Property.GetInteger("index");

                }
                if (_Property.ContainsKey("width")) {

                    this.Width =  _Property.GetInteger("width");

                }
                if (_Property.ContainsKey("scale")) {

                    this.Scale =  _Property.GetInteger("scale");

                }
                if (_Property.ContainsKey("reference")) {

                    this.Reference = _Property.GetValue("reference");

                }
                if (_Property.ContainsKey("enableMode")) {

                    this.EnableMode = _Property.GetValue("enableMode");

                }
                if (_Property.ContainsKey("alignName")) {

                    this.AlignName = _Property.GetValue("alignName");

                }
                if (_Property.ContainsKey("options")) {

                    this.Options = _Property.GetValue("options");

                }
                if (_Property.ContainsKey("nonPrintable")) {

                    this.NonPrintable = _Property.GetBoolean("nonPrintable");

                }
                if (_Property.ContainsKey("axis")) {

                    this.Axis = _Property.GetValue("axis");

                }

            }

            internal void Write(StringBuilder writer)
            {
                _Property = new JSONObject();

                _Property.SetValue("name"        , this.Name);
                _Property.SetValue("caption"     , this.Caption);
                _Property.SetValue("dataType"    , this.DataType);
                _Property.SetInteger("width"     , this.Width);
                _Property.SetInteger("scale"     , this.Scale);
                _Property.SetValue("enableMode"  , this.EnableMode);
                _Property.SetValue("options"     , this.Options);
                if (this.Description!=""){
                    _Property.SetValue("description" , this.Description);
                }
                if (this.ColumnName!=""){
                    _Property.SetValue("columnName" , this.ColumnName);
                }
                if (this.Reference!=""){
                    _Property.SetValue("reference" , this.Reference);
                }
                if (this.AlignName!=""){
                    _Property.SetValue("alignName" , this.AlignName);
                }
                _Property.SetValue("className" , this.ClassName);

                if (this.Axis!=""){
                    _Property.SetValue("axis" , this.Axis);
                }
                _Property.SetBoolean("nonPrintable" , this.NonPrintable);
                _Property.SetInteger("index"        , this.Index);

                _Property.Write(writer);

            }
        }
}
