using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.IO;

namespace Volte.Data.Json
{
    internal class RowComparer : IComparer<Row> {
        private int     _column_ndx1 = -1;
        private int     _column_ndx2 = -1;
        private int     _column_ndx3 = -1;
        private int     _column_ndx4 = -1;
        private int     _column_ndx5 = -1;
        private int     _column_ndx6 = -1;
        private int     _direct1     = 0;
        private int     _direct2     = 0;
        private int     _direct3     = 0;
        private int     _direct4     = 0;
        private int     _direct5     = 0;
        private int     _direct6     = 0;
        private Columns _Columns;

        public RowComparer(Columns cColumns, string Name)
        {
            string[] aName = Name.Split(',');
            _Columns = cColumns;
            string _Name = "";

            if (aName.Length >= 1) {
                _Name = aName[0].Trim();

                if (_Name.IndexOf("^") >= 0) {
                    _direct1 = 1;
                    _Name.Replace("^", "");
                }

                _column_ndx1 = _Columns.Ordinal(_Name);
            }

            if (aName.Length >= 2) {

                _Name = aName[1].Trim();

                if (_Name.IndexOf("^") >= 0) {
                    _direct2 = 1;
                    _Name.Replace("^", "");
                }

                _column_ndx2 = _Columns.Ordinal(_Name);
            }

            if (aName.Length >= 3) {

                _Name = aName[2].Trim();

                if (_Name.IndexOf("^") >= 0) {
                    _direct3 = 1;
                    _Name.Replace("^", "");
                }

                _column_ndx3 = _Columns.Ordinal(_Name);
            }

            if (aName.Length >= 4) {
                _Name = aName[3].Trim();

                if (_Name.IndexOf("^") >= 0) {
                    _direct4 = 1;
                    _Name.Replace("^", "");
                }

                _column_ndx4 = _Columns.Ordinal(_Name);
            }

            if (aName.Length >= 5) {
                _Name = aName[4].Trim();

                if (_Name.IndexOf("^") >= 0) {
                    _direct5 = 1;
                    _Name.Replace("^", "");
                }

                _column_ndx5 = _Columns.Ordinal(_Name);
            }

            if (aName.Length >= 6) {
                _Name = aName[5].Trim();

                if (_Name.IndexOf("^") >= 0) {
                    _direct6 = 1;
                    _Name.Replace("^", "");
                }

                _column_ndx6 = _Columns.Ordinal(_Name);
            }
        }

        private int CompareIt(int ndx, Row r1, Row r2)
        {
            int _Compare = 0;

            if (ndx >= 0) {
                string _column_type = _Columns[ndx].DataType;

                if (_column_type == "datetime") {

                    DateTime d1 = r1.getDateTime(ndx);
                    DateTime d2 = r2.getDateTime(ndx);

                    _Compare = d1.CompareTo(d2);

                } else if (_column_type == "decimal") {

                    decimal d1 = decimal.Parse(r1[ndx].Value.ToString());
                    decimal d2 = decimal.Parse(r2[ndx].Value.ToString());

                    _Compare = d1.CompareTo(d2);

                } else {

                    _Compare = string.Compare(r1[ndx].Value.ToString(), r2[ndx].Value.ToString(), true);
                }
            } else {
                _Compare = string.Compare(r1[0].Value.ToString(), r2[0].Value.ToString(), true);
            }

            return _Compare;
        }

        public int Compare(Row r1, Row r2)
        {
            int _Compare = 0;

            if (_column_ndx1 >= 0) {

                if (_direct1 == 0) {
                    _Compare = CompareIt(_column_ndx1, r1, r2);
                } else {
                    _Compare = CompareIt(_column_ndx1, r2, r1);
                }

                if (_Compare == 0 && _column_ndx2 >= 0) {
                    if (_direct2 == 0) {
                        _Compare = CompareIt(_column_ndx2, r1, r2);
                    } else {
                        _Compare = CompareIt(_column_ndx2, r2, r1);
                    }
                }

                if (_Compare == 0 && _column_ndx3 >= 0) {
                    if (_direct3 == 0) {
                        _Compare = CompareIt(_column_ndx3, r1, r2);
                    } else {
                        _Compare = CompareIt(_column_ndx3, r2, r1);
                    }
                }

                if (_Compare == 0 && _column_ndx4 >= 0) {
                    if (_direct4 == 0) {
                        _Compare = CompareIt(_column_ndx4, r1, r2);
                    } else {
                        _Compare = CompareIt(_column_ndx4, r2, r1);
                    }
                }

                if (_Compare == 0 && _column_ndx5 >= 0) {
                    if (_direct5 == 0) {
                        _Compare = CompareIt(_column_ndx5, r1, r2);
                    } else {
                        _Compare =  CompareIt(_column_ndx5, r2, r1);
                    }
                }

                if (_Compare == 0 && _column_ndx6 >= 0) {
                    if (_direct6 == 0) {
                        _Compare = CompareIt(_column_ndx6, r1, r2);
                    } else {
                        _Compare = CompareIt(_column_ndx6, r2, r1);
                    }
                }

            } else {
                if (_direct1 == 0) {
                    _Compare = string.Compare(r2[0].Value.ToString(), r1[0].Value.ToString(), true);
                } else {
                    _Compare = string.Compare(r1[0].Value.ToString(), r2[0].Value.ToString(), true);
                }
            }

            return _Compare;
        }
    }
}
