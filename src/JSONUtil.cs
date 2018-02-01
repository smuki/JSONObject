using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Security.Cryptography;
using System.IO.Compression;

using Volte.Utils;

namespace Volte.Data.Json
{

    public class JSONUtil {
        const string ZFILE_NAME = "JSONUtil";

        public JSONUtil()
        {

        }

        public static DataTable ToDataTable(string name , JSONTable _JSONTable , string SortBy="")
        {

            bool bRec    = _JSONTable.Variable.GetBoolean("bHasRec");
            DataTable dt = new DataTable(name);

            DataColumn column2 = new DataColumn("nRecNo") ;
            column2.DataType   = typeof(int);
            if (bRec){
                dt.Columns.Add(column2);
            }
            string sSortBy="";

            foreach (AttributeMapping _AttributeMapping in _JSONTable.Fields) {
                column2 = new DataColumn(_AttributeMapping.Hash) {
                    Caption = _AttributeMapping.Caption
                };
                string type = _AttributeMapping.DataType;
                type        = type.Replace("System.", "");
                type        = type.ToLower();
                if (SortBy==_AttributeMapping.Name){
                    sSortBy=_AttributeMapping.Name;
                }

                if (type == "decimal") {
                    column2.DataType = System.Type.GetType("System.Decimal");
                } else if (type == "datetime") {
                    column2.DataType = typeof(DateTime);
                } else if (type == "int32" || type == "integer") {
                    column2.DataType = typeof(int);
                } else {
                    column2.DataType = System.Type.GetType("System.String");
                }

                dt.Columns.Add(column2);
            }

            _JSONTable.Open();
            int ndx = 1;
            int offset = 0;

            while (!_JSONTable.EOF) {

                DataRow _dataRow = dt.NewRow();
                if (bRec){
                    _dataRow[0] = ndx;
                    offset = 1;

                }

                for (int i = 0; i < _JSONTable.Fields.Count; i++) {
                    string type = _JSONTable.Fields[i].DataType.ToLower();

                    if (type == "decimal") {
                        _dataRow[i + offset] = _JSONTable.GetDecimal(i);
                    } else if (type == "datetime") {
                        if (_JSONTable.GetDateTime2(i) == null || _JSONTable.GetDateTime(i)<= Util.DateTime_MinValue) {
                            _dataRow[i + offset] = DBNull.Value;
                        } else {
                            _dataRow[i + offset] = _JSONTable.GetDateTime2(i);
                        }
                    } else {
                        _dataRow[i + offset] = _JSONTable[i];
                    }
                }

                dt.Rows.Add(_dataRow);
                ndx++;
                _JSONTable.MoveNext();
            }
            if (sSortBy!=""){
                DataView dv = dt.DefaultView;
                dv.Sort = sSortBy;
                return dv.ToTable();
            }else{
                return dt;
            }
        }

        public static void ToObjects(StringBuilder writer , JSONTable _JSONTable)
        {

            writer.AppendLine("[");

            _JSONTable.MoveFirst();
            int rec=0;

            while (!_JSONTable.EOF) {
                if (rec > 0) {
                    writer.Append(",");

                    writer.AppendLine("");
                }
                writer.AppendLine("{");

                for (int i = 0; i < _JSONTable.Fields.Count; i++) {

                    string _Name = _JSONTable.Fields[i].Hash;
                    string type  = _JSONTable.Fields[i].DataType.ToLower();

                    if (i > 0) {
                        writer.Append(",");
                        writer.AppendLine("");
                    }

                    writer.Append("\"" + _Name + "\":");

                    if (type== "decimal" || type== "integer") {
                        writer.Append(_JSONTable.GetDecimal(i).ToString());
                    } else if (type == "datetime") {

                        if (_JSONTable.GetDateTime(i) <= Util.DateTime_MinValue) {
                            writer.Append("\"\"");
                        } else {
                            writer.Append("\"");
                            writer.Append(_JSONTable.GetDateTime(i).ToString("yyyyMMddhhmmss"));
                            writer.Append("\"");
                        }

                    } else if (type == "boolean") {
                        writer.Append(_JSONTable.GetValue(i).ToLower());
                    } else {
                        writer.Append("\"");
                        Util.EscapeString(writer, _JSONTable.GetValue(i));
                        writer.Append("\"");
                    }

                }
                writer.Append(",");
                writer.Append("\"k\":");
                writer.Append("\"");
                Util.EscapeString(writer, _JSONTable.Reference.GetValue("k"));
                writer.Append("\"");
                writer.AppendLine("}");

                rec++;
                _JSONTable.MoveNext();
            }

            writer.AppendLine("");
            writer.Append("]");

            return ;
        }

        public static JSONArray ToJSONObject(string name , JSONTable _JSONTable)
        {

            JSONArray _JSONArray  = new JSONArray();

            _JSONTable.MoveFirst();

            while (!_JSONTable.EOF) {

                JSONObject _Item = new JSONObject();

                for (int i = 0; i < _JSONTable.Fields.Count; i++) {

                    string _Name = _JSONTable.Fields[i].Hash;
                    string type  = _JSONTable.Fields[i].DataType.ToLower();

                    if (type == "decimal") {
                        _Item.SetValue(_Name , _JSONTable.GetDecimal(i));
                    } else if (type == "datetime") {
                        _Item.SetValue(_Name , _JSONTable.GetDateTime2(i));
                    } else {
                        if (_JSONTable.GetAttribute(i,"sCode")!=null){
                            _Item.SetValue(_Name +"_Code" , _JSONTable.GetAttribute(i,"sCode"));
                        }

                        _Item.SetValue(_Name , _JSONTable[i]);
                    }
                }

                _JSONArray.Add(_Item);

                _JSONTable.MoveNext();
            }

            return _JSONArray;
        }
    }
}
