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

                foreach(string s in _Property.Names){
                    if (s.ToLower()=="datatype") {

                        this.DataType = _Property.GetValue("dataType");

                    }else if (s.ToLower()=="name") {

                        this.Name = _Property.GetValue("name");

                    }else if (s.ToLower()=="caption") {

                        this.Caption = _Property.GetValue("caption");

                    }else if (s.ToLower()=="description") {

                        this.Description = _Property.GetValue("description");

                    }else if (s.ToLower()=="columnname") {

                        this.ColumnName = _Property.GetValue("columnName");

                    }else if (s.ToLower()=="index") {

                        this.Index =  _Property.GetInteger("index");

                    }else if (s.ToLower()=="width") {

                        this.Width =  _Property.GetInteger("width");

                    }else if (s.ToLower()=="scale") {

                        this.Scale =  _Property.GetInteger("scale");

                    }else if (s.ToLower()=="reference") {

                        this.Reference = _Property.GetValue("reference");

                    }else if (s.ToLower()=="enablemode") {

                        this.EnableMode = _Property.GetValue("enableMode");

                    }else if (s.ToLower()=="alignname") {

                        this.AlignName = _Property.GetValue("alignName");

                    }else if (s.ToLower()=="options") {

                        this.Options = _Property.GetValue("options");
                    }else if (s.ToLower()=="format") {

                        this.Format = _Property.GetValue("format");

                    }else if (s.ToLower()=="hidden") {

                        this.Hidden = _Property.GetBoolean("hidden");

                    }else{

                        this.Props[s] = _Property[s];

                    }
                }
                if (!string.IsNullOrEmpty(this.Name)) {

                    this.Hash = this.Name.Replace("." , "_");

                }

            }

            internal void Write(StringBuilder writer)
            {
                _Property = new JSONObject();

                _Property.SetValue("name"        , this.Name);
                _Property.SetValue("caption"     , this.Caption);
                if (this.DataType==""){
                    _Property.SetValue("dataType"    , "string");
                }else{
                    _Property.SetValue("dataType"    , this.DataType);
                }
                _Property.SetInteger("width" , this.Width);
                _Property.SetInteger("scale" , this.Scale);
                if (!string.IsNullOrEmpty(this.EnableMode)){
                    _Property.SetValue("enableMode"  , this.EnableMode);
                }
                if (!string.IsNullOrEmpty(this.Options)){
                    _Property.SetValue("options"     , this.Options);
                }
                if (this.Description!=""){
                    _Property.SetValue("description" , this.Description);
                }
                if (this.ColumnName!=""){
                    _Property.SetValue("columnName" , this.ColumnName);
                }
                if (this.Reference!=""){
                    _Property.SetValue("reference" , this.Reference);
                }
                if (this.Format!=""){
                    _Property.SetValue("format" , this.Format);
                }
                if (this.AlignName!=""){
                    _Property.SetValue("alignName" , this.AlignName);
                }

                _Property.SetBoolean("hidden" , this.Hidden);
                _Property.SetInteger("index"  , this.Index);
                foreach(string s in this.Props.Names){
                    _Property[s] = this.Props[s];
                }

                _Property.Write(writer);

            }
        }
}
