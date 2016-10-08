using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Volte.Data.Json
{
    [Serializable]
        public class JSONTable {
            const string ZFILE_NAME = "JSONTable";
            private readonly StringBuilder _Fields    = new StringBuilder();

            public JSONTable()
            {
                _Columns  = new Columns();
                _Variable = new JSONObject();
                _Pointer  = -1;
            }

            internal void Read(Lexer _Lexer)
            {
                _Columns  = new Columns();
                _Variable = new JSONObject();

                _Lexer.SkipWhiteSpace();

                if (_Lexer.Current != '{') {
                    throw new ArgumentException("Invalid element", "element");
                }

                _Lexer.NextToken();

                if (_Lexer.Current != '}') {
                    for (;;) {
                        string name = _Lexer.ParseName();

                        if (name == "columns") {
                            _Columns.Read(_Lexer);
                        } else if (name == "vars") {
                            _Variable.Read(_Lexer);
                        } else if (name == "rows") {
                            _Lexer.SkipWhiteSpace();

                            if (_Lexer.Current == '[') {
                                _Lexer.NextToken();

                                for (;;) {
                                    Row row1 = new Row(_Columns.Count);
                                    row1.Read(_Lexer);
                                    _rows.Add(row1);

                                    _Lexer.SkipWhiteSpace();

                                    if (_Lexer.Current == ',') {
                                        _Lexer.NextToken();
                                    } else {
                                        break;
                                    }
                                }
                            }
                        }

                        if (_Lexer.Current == ',') {
                            _Lexer.NextToken();
                        } else {
                            break;
                        }
                    }
                }
            }

            internal void Write(StringBuilder  writer)
            {
                writer.AppendLine("{");

                if (_Variable == null) {

                    _Variable = new JSONObject();
                }

                _Variable.SetValue("RecordCount" , this.RecordCount);

                writer.AppendLine("\"vars\":");
                _Variable.Write(writer);
                writer.AppendLine("");
                writer.AppendLine(",");

                if (_Columns != null) {
                    _Columns.Write(writer);
                    writer.AppendLine(",");
                }

                if (this._rows != null) {
                    writer.AppendLine("\"rows\":");
                    writer.AppendLine("[");
                    int rec = 0;

                    if (!_StructureOnly) {
                        foreach (Row _row in _rows) {
                            if (rec > 0) {
                                writer.AppendLine(",");
                            }

                            if (rec % 100 == 0) {
                                _row_index[rec.ToString()] = writer.Length;
                            }

                            _row.Write(writer);
                            rec++;
                        }
                    }

                    _StructureOnly = false;

                    writer.AppendLine("]");
                }

                writer.AppendLine("}");
            }

            public DataTable GetDataTable()
            {
                return GetDataTable("");
            }

            public DataTable GetDataTable(string name)
            {

                DataTable dt = new DataTable(name);

                DataColumn column2 = new DataColumn("nRecNo") ;
                column2.DataType = typeof(int);
                dt.Columns.Add(column2);

                foreach (AttributeMapping _AttributeMapping in this.Fields) {
                    column2 = new DataColumn(_AttributeMapping.Name.Replace(".", "_")) {
                        Caption = _AttributeMapping.Caption
                    };
                    string type = _AttributeMapping["TP_CODE"].ToString();
                    type        = type.Replace("System.", "");
                    type        = type.ToLower();

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

                this.Open();
                int ndx = 1;

                while (!this.EOF) {

                    DataRow _dataRow = dt.NewRow();

                    _dataRow[0] = ndx;

                    for (int i = 0; i < this.Fields.Count; i++) {
                        string type = this.Fields[i]["TP_CODE"].ToString().ToLower();

                        if (type == "decimal") {
                            _dataRow[i + 1] = this.GetDecimal(i);
                        } else if (type == "datetime") {
                            if (this.GetDateTime2(i) == null) {
                                _dataRow[i + 1] = DBNull.Value;
                            } else {
                                _dataRow[i + 1] = this.GetDateTime2(i);
                            }
                        } else {
                            _dataRow[i + 1] = this[i];
                        }
                    }

                    dt.Rows.Add(_dataRow);
                    ndx++;
                    this.MoveNext();
                }

                return dt;
            }

            public void Parser(string _string)
            {
                Lexer oLexer = new Lexer(_string);
                this.Read(oLexer);
            }

            public string ToString(bool xx = false)
            {
                _StructureOnly = xx;
                _s.Length    = 0;
                this.Write(_s);
                return _s.ToString();
            }

            public override string ToString()
            {
                _s.Length    = 0;
                this.Write(_s);
                return _s.ToString();
            }

            public string IndexString()
            {
                _s.Length    = 0;
                _row_index.Write(_s);
                return _s.ToString();
            }

            public void AddNew()
            {
                _Draft   = true;
                _Pointer = 0;
                _Row     = new Row(_Columns.Count);
                _Readed  = true;
            }

            public void Revert()
            {
                if (_Draft) {
                    _Row    = new Row(_Columns.Count);
                    _Draft  = false;
                    _Readed = false;
                }
            }

            public void Update()
            {
                if (_Draft) {
                    _rows.Add(_Row);
                } else {
                    _rows[_Pointer] = _Row;
                }

                _Draft = false;
            }

            public void Open()
            {
                _Pointer = 0;

                if (_Pointer < _rows.Count) {
                    _Row = _rows[_Pointer];
                } else {
                    _rows = null;
                }

                _Readed = true;
            }

            public void Close()
            {
                _rows     = null;
                _Readed   = false;
                _Draft    = false;
                _Row      = null;
                _Columns  = new Columns();
                _Variable = new JSONObject();
                _Pointer  = -1;
            }

            public object this[string name]
            {
                get {
                    int _Ordinal = _Columns.Ordinal(name);

                    if (_Ordinal == -1) {
                        throw new ArgumentException("Invalid column name", name);
                    }

                    if (!_Readed) {
                        _Readed = true;
                        _Row    = _rows[_Pointer];
                    }

                    return _Row[_Ordinal].Text;
                } set {
                    if (!object.Equals(value, null)) {
                        int _Ordinal = _Columns.Ordinal(name);

                        if (_Ordinal == -1) {
                            throw new ArgumentException("Invalid column name", name);
                        }

                        _Row[_Ordinal] = new Cell(value);
                    }
                }
            }

            public object this[int i]
            {
                get {
                    if (!_Readed) {

                        _Readed = true;
                        _Row    = _rows[_Pointer];
                    }

                    return _Row[i].Text;
                } set {
                    if (i < 0 || i >= _Columns.Count) {
                        throw new ArgumentException("Invalid column index + _Columns=" + _Columns.Count, i.ToString());
                    }

                    _Row[i] = new Cell(value);
                }
            }

            public bool GetBoolean(string Name)
            {
                return this.GetBoolean(_Columns.Ordinal(Name));
            }

            public bool GetBoolean(int Index)
            {
                object cValue = this[Index];


                if (cValue is bool) {
                    return (bool) cValue;
                } else if (cValue.Equals("Y") || cValue.Equals("y")) {
                    return true;
                } else if (cValue.Equals("N") || cValue.Equals("n")) {
                    return false;
                } else {
                    return Util.ToBoolean(cValue);
                }
            }

            public decimal GetDecimal(int Index)
            {
                object cValue = this[Index];

                if (string.IsNullOrEmpty(cValue.ToString())) {
                    return 0;
                }

                return Util.ToDecimal(cValue);
            }

            public decimal GetDecimal(string Name)
            {

                return this.GetDecimal(_Columns.Ordinal(Name));

            }

            public int GetInt32(int i)
            {
                return GetInteger(i);
            }

            public int GetInteger(int i)
            {
                return Util.ToInt32(this[i]);
            }

            public int GetInteger(string Name)
            {
                return Util.ToInt32(this[Name]);
            }

            public string GetString(int i)
            {
                return GetValue(i);
            }

            public string GetValue(int i)
            {
                object _obj = this[i];

                if (_obj == null) {
                    return "";
                } else {
                    return _obj.ToString();
                }
            }

            public string GetValue(string Name)
            {
                object _obj = this[Name];

                if (_obj == null) {
                    return "";
                } else {
                    return _obj.ToString();
                }
            }

            public DateTime GetDateTime(string Name)
            {
                int _Ordinal = _Columns.Ordinal(Name);
                return GetDateTime(_Ordinal);
            }

            public DateTime? GetDateTime2(string Name)
            {
                int _Ordinal = _Columns.Ordinal(Name);
                return GetDateTime(_Ordinal);
            }

            public DateTime GetDateTime(int i)
            {
                return Util.ToDateTime(this[i]);
            }

            public DateTime? GetDateTime2(int i)
            {
                return Util.ToDateTime(this[i]);
            }

            public void SetValue(string Name, object value)
            {
                this[Name] = value;
            }

            public void MoveFirst()
            {
                _Pointer = 0;
                _Readed   = false;
            }

            public void MoveLast()
            {
                _Pointer = _rows.Count - 1;
                _Readed   = false;
            }

            public void Locate(int cPoint)
            {
                _Pointer = cPoint;
                _Readed = false;
            }

            public void MovePrev()
            {
                _Pointer--;
                _Readed = false;
            }

            public void MoveNext()
            {
                _Pointer++;
                _Readed = false;
            }

            public void Declare(string name, string caption, string type, int width = 12, int scale = 2)
            {
                Column _Column    = new Column();
                _Column.Name      = name;
                _Column.Caption   = caption;
                _Column.DataType  = type;
                _Column.Scale     = scale;
                _Column.Width     = width;
                _Column.CellWidth = width * 12;
                _Columns.Add(_Column);
            }

            public void Declare(AttributeMapping _DataField)
            {
                Column _Column       = new Column();
                _Column.Name         = _DataField.Name.Replace(".", "_");
                _Column.Caption      = _DataField.Caption;
                _Column.Scale        = _DataField.Scale;
                _Column.Width        = _DataField.Width;
                _Column.CellWidth    = _DataField.CellWidth;
                _Column.Status       = _DataField.Status;
                _Column.EnableMode   = _DataField.EnableMode;
                _Column.Reference    = _DataField.Reference;
                _Column.TypeChar     = _DataField.TypeChar;
                _Column.Compulsory   = _DataField.Compulsory;
                _Column.NonPrintable = _DataField.NonPrintable;
                _Column.DataType     = _DataField.DataType;
                _Column.DataBand     = _DataField.DataBand;
                _Column.AlignName    = _DataField.AlignName;
                _Columns.Add(_Column);
            }

            public void DeclareStart()
            {

            }

            public int Ordinal(string name)
            {
                return _Columns.Ordinal(name);
            }

            public void DeclareFinal()
            {
            }

            public void Sort(string Name)
            {
                RowComparer _RowComparer = new RowComparer(_Columns, Name);
                _rows.Sort(_RowComparer.Compare);
            }

            public JSONObject Reference
            {
                get {
                    if (!_Readed) {

                        _Readed = true;
                        _Row    = _rows[_Pointer];
                    }


                    if (_Row.Reference==null){
                        _Row.Reference = new JSONObject();
                    }
                    return _Row.Reference;
                } set {
                    _Row.Reference = value;
                }
            }

            public int RecordCount
            {
                get {
                    if (_rows == null) {
                        return -1;
                    } else {
                        return _rows.Count;
                    }
                }
            }

            // Properties
            public bool EOF
            {
                get {
                    if (_rows == null) {
                        return true;
                    }

                    return _Pointer >= _rows.Count;
                }
            }

            public bool BOF            { get { return _Pointer < 0;    }  }
            public JSONObject Variable { get { return _Variable;       }  }
            public JSONObject Index    { get { return _row_index;      }  }
            public List<Column> Fields { get { return _Columns.Fields; }  }

            private Row _Row;
            private Columns _Columns;
            private JSONObject _Variable      = new JSONObject();
            private JSONObject _row_index     = new JSONObject();
            private List<Row>  _rows          = new List<Row>();
            private readonly StringBuilder _s = new StringBuilder();
            private bool _Draft               = false;
            private bool _Readed              = false;
            private bool _StructureOnly       = false;
            private int _Pointer              = -1;
        }
}
