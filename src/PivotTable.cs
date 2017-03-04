using System;
using System.Data;
using System.Collections;

using Volte.Utils;
using Volte.Data.Json;

namespace Volte.Data.Json
{
    /// <summary>
    /// Create simple and advanced pivot reports.
    /// </summary>
    public class PivotTable {

        const string ZFILE_NAME = "PivotTable";
        private JSONTable _DataTable;

        public PivotTable(JSONTable dataTable)
        {
            _DataTable = dataTable;
        }

        private string[] FindValues(string xAxisField, string xAxisValue, string yAxisField, string yAxisValue, string[] zAxisFields)
        {
            int zAxis = zAxisFields.Length;

            if (zAxis < 1)
                zAxis++;

            string[] zAxisValues = new string[zAxis];

            //set default values
            for (int i = 0; i <= zAxisValues.GetUpperBound(0); i++) {
                zAxisValues[i] = "0";
            }

            _DataTable.MoveFirst();
            while (!_DataTable.EOF) {
                if (_DataTable.GetValue(xAxisField) == xAxisValue && _DataTable.GetValue(yAxisField) == yAxisValue) {
                    for (int z = 0; z < zAxis; z++) {
                        zAxisValues[z] = _DataTable.GetValue(zAxisFields[z]);
                    }

                    break;
                }
                _DataTable.MoveNext();
            }

            return zAxisValues;
        }

        public JSONObject Chart(JSONTable _Data,JSONObject parameters)
        {
            JSONObject _json = new JSONObject();
            JSONArray _label = new JSONArray();
            JSONArray _datasets = new JSONArray();

            int i=0;
            _json.SetValue("xAxes" , parameters.GetValue("xAxes"));
            _json.SetValue("yAxes" , parameters.GetValue("yAxes"));

            _Data.MoveFirst();
            int rec=1;
            if (parameters.GetValue("yAxes")==""){
                JSONObject _record = new JSONObject();

                JSONArray _data  = new JSONArray();
                JSONArray _color = new JSONArray();
                foreach (AttributeMapping Fields in _Data.Fields){
                    if (i==0){
                        _record.SetValue("label" , "");
                    }
                    i++;
                }

                while (!_Data.EOF) {
                    _label.Add(_Data.GetValue(0));
                    _data.Add(_Data.GetDecimal(1));
                    _color.Add(parameters.GetValue("backgroundColor_"+rec));
                    rec++;

                    _Data.MoveNext();
                }
                _record.SetValue("data"            , _data);
                _record.SetValue("fill"            , parameters.GetBoolean("fill"));
                _record.SetValue("backgroundColor" , _color);
                _record.SetValue("borderColor"     , parameters.GetValue("backgroundColor_"+rec));
                //_record.SetValue("labels" , _label);

                _json.SetValue("labels" , _label);
                _datasets.Add(_record);

            }else{

                foreach (AttributeMapping Fields in _Data.Fields){
                    if (i>0){
                        _label.Add(Fields.Caption);
                    }
                    i++;
                }
                _json.SetValue("labels" , _label);

                while (!_Data.EOF) {

                    JSONObject _record = new JSONObject();
                    _record.SetValue("label" , _Data.GetValue(0));

                    JSONArray _data = new JSONArray();

                    i=0;

                    foreach (AttributeMapping Fields in _Data.Fields){
                        if (i>0){
                            _data.Add(_Data.GetDecimal(i));
                        }
                        i++;
                    }
                    _record.SetValue("data"            , _data);
                    _record.SetValue("fill"            , parameters.GetBoolean("fill"));
                    _record.SetValue("backgroundColor" , parameters.GetValue("backgroundColor_"+rec));
                    _record.SetValue("borderColor"     , parameters.GetValue("backgroundColor_"+rec));

                    _datasets.Add(_record);
                    rec++;

                    _Data.MoveNext();
                }
            }

            _json.SetValue("datasets" , _datasets);

            return _json;

        }

        private string FindValue(string xAxisField, string xAxisValue, string yAxisField, string yAxisValue, string zAxisField)
        {
            string zAxisValue = "";

            _DataTable.MoveFirst();
            while (!_DataTable.EOF) {
                if (_DataTable.GetValue(xAxisField) == xAxisValue && _DataTable.GetValue(yAxisField) == yAxisValue) {
                    zAxisValue = _DataTable.GetValue(zAxisField);
                    break;
                }
                _DataTable.MoveNext();
            }

            return zAxisValue;
        }

        /// <summary>
        /// Creates an advanced 3D Pivot table.
        /// </summary>
        /// <param name="xAxisField">The main heading at the top of the report.</param>
        /// <param name="yAxisField">The heading on the left of the report.</param>
        /// <param name="zAxisFields">The sub heading at the top of the report.</param>
        /// <param name="mainColumnName">Name of the column in xAxis.</param>
        /// <param name="columnTotalName">Name of the column with the totals.</param>
        /// <param name="rowTotalName">Name of the row with the totals.</param>
        /// <param name="zAxisFieldsNames">Name of the columns in the zAxis.</param>
        /// <returns>HtmlTable Control.</returns>
        public JSONTable Generate(string xAxisField, string yAxisField, string[] zAxisFields, string mainColumnName, string columnTotalName, string rowTotalName, string[] zAxisFieldsNames)
        {

            /*
             * The x-axis is the main horizontal row.
             * The z-axis is the sub horizontal row.
             * The y-axis is the left vertical column.
             */

            //get distinct xAxisFields
            ArrayList xAxis = new ArrayList();

            _DataTable.MoveFirst();
            while (!_DataTable.EOF) {
                if (!xAxis.Contains(_DataTable.GetValue(xAxisField))){
                    xAxis.Add(_DataTable.GetValue(xAxisField));
                }
                _DataTable.MoveNext();
            }

            //get distinct yAxisFields
            ArrayList yAxis = new ArrayList();

            _DataTable.MoveFirst();
            while (!_DataTable.EOF) {
                if (!yAxis.Contains(_DataTable.GetValue(yAxisField))){
                    yAxis.Add(_DataTable.GetValue(yAxisField));
                }
                _DataTable.MoveNext();
            }

            //create a 2D array for the y-axis/z-axis fields
            int zAxis = zAxisFields.Length;

            if (zAxis < 1)
                zAxis = 1;

            string[,] matrix = new string[(xAxis.Count * zAxis), yAxis.Count];

            for (int y = 0; y < yAxis.Count; y++) { //loop thru y-axis fields
                //rows
                for (int x = 0; x < xAxis.Count; x++) { //loop thru x-axis fields
                    //main columns
                    //get the z-axis values
                    string[] zAxisValues = FindValues(xAxisField, Convert.ToString(xAxis[x])
                                                      , yAxisField, Convert.ToString(yAxis[y]), zAxisFields);

                    for (int z = 0; z < zAxis; z++) { //loop thru z-axis fields
                        //sub columns
                        matrix[(((x + 1) * zAxis - zAxis) + z), y] = zAxisValues[z];
                    }
                }
            }

            //calculate totals for the y-axis
            decimal[] yTotals = new decimal[(xAxis.Count * zAxis)];

            for (int col = 0; col < (xAxis.Count * zAxis); col++) {
                yTotals[col] = 0;

                for (int row = 0; row < yAxis.Count; row++) {
                    yTotals[col] += Convert.ToDecimal("0" + matrix[col, row]);
                }
            }

            //calculate totals for the x-axis
            decimal[,] xTotals = new decimal[zAxis, (yAxis.Count + 1)];

            for (int y = 0; y < yAxis.Count; y++) { //loop thru the y-axis
                int zCount = 0;

                for (int z = 0; z < (zAxis * xAxis.Count); z++) { //loop thru the z-axis
                    xTotals[zCount, y] += Convert.ToDecimal("0" + matrix[z, y]);

                    if (zCount == (zAxis - 1))
                        zCount = 0;
                    else
                        zCount++;
                }
            }

            for (int xx = 0; xx < zAxis; xx++) { //Grand Total
                for (int xy = 0; xy < yAxis.Count; xy++) {
                    xTotals[xx, yAxis.Count] += xTotals[xx, xy];
                }
            }

            //Build HTML Table
            //Append main row (x-axis)
            JSONTable table = new JSONTable();

            AttributeMapping columnYTitle = new AttributeMapping();
            columnYTitle.Name             = mainColumnName;
            columnYTitle.Width            = 8;
            columnYTitle.Caption          = "";
            table.Declare(columnYTitle);

            for (int x = 0; x <= xAxis.Count; x++) { //loop thru x-axis + 1
                if (x < xAxis.Count) {
                    for (int z = 0; z < zAxis; z++) {

                        AttributeMapping column = new AttributeMapping();
                        column.Name             = Convert.ToString(xAxis[x] + " - " + zAxisFieldsNames[z]);
                        column.Width            = 8;
                        column.DataType         = "decimal";
                        column.TypeChar         = "n";
                        column.Scale            = 0;
                        column.Caption          = Convert.ToString(xAxis[x] + " - " + zAxisFieldsNames[z]);
                        table.Declare(column);

                    }
                } else {
                    for (int z = 0; z < zAxis; z++) {

                        AttributeMapping column = new AttributeMapping();
                        column.Name             = columnTotalName + " - " + zAxisFieldsNames[z];
                        column.Width            = 8;
                        column.DataType         = "decimal";
                        column.TypeChar         = "n";
                        column.Scale            = 0;
                        column.Caption          = columnTotalName + " - " + zAxisFieldsNames[z];
                        table.Declare(column);

                    }
                }
            }


            //Append table items from matrix
            for (int y = 0; y < yAxis.Count; y++) { //loop thru y-axis
                table.AddNew();

                for (int z = 0; z <= (zAxis * xAxis.Count); z++) { //loop thru z-axis + 1
                    if (z == 0) {
                        table.SetValue(z , Convert.ToString(yAxis[y]));
                    } else {
                        table.SetValue(z , Convert.ToString(matrix[(z - 1) , y]));
                    }
                }

                //append x-axis grand totals
                for (int z = zAxis * xAxis.Count; z < zAxis + (zAxis * xAxis.Count); z++) {
                    table.SetValue(z +1 , Convert.ToString(xTotals[z - (zAxis * xAxis.Count) , y]));

                }

                table.Update();
            }

            if (rowTotalName!=""){
                //append y-axis totals
                table.AddNew();

                for (int x = 0; x <= (zAxis * xAxis.Count); x++) {
                    if (x == 0){
                        table.SetValue(0 , rowTotalName);
                    } else{
                        table.SetValue(x , Convert.ToString(yTotals[x - 1]));
                    }
                }

                //append x-axis/y-axis totals
                for (int z = 0; z < zAxis; z++) {
                    table.SetValue(table.Fields.Count - zAxis + z, Convert.ToString(xTotals[z, xTotals.GetUpperBound(1)]));
                }

                table.Update();
            }

            return table;
        }

        public JSONTable Generate(string xAxisField, string yAxisField, string zAxisField, string mainColumnName)
        {
            return Generate(xAxisField, yAxisField, new string[0], new string[0], zAxisField, mainColumnName, "", "");
        }

        public JSONTable Generate(string xAxisField, string yAxisField, string zAxisField, string mainColumnName, string columnTotalName, string rowTotalName)
        {
            return Generate(xAxisField, yAxisField, new string[0], new string[0], zAxisField, mainColumnName, columnTotalName, rowTotalName);
        }

        /// <summary>
        /// Creates a simple 3D Pivot Table.
        /// </summary>
        /// <param name="xAxisField">The heading at the top of the table.</param>
        /// <param name="yAxisField">The heading to the left of the table.</param>
        /// <param name="yAxisInfoFields">Other columns that we want to show on the y axis.</param>
        /// <param name="yAxisInfoFieldsNames">Title of the additionnal columns on y axis.</param>
        /// <param name="zAxisField">The item value field.</param>
        /// <param name="mainColumnName">Title of the main column</param>
        /// <param name="columnTotalName">Title of the total column</param>
        /// <param name="rowTotalName">Title of the row column</param>
        /// <returns></returns>
        public JSONTable Generate(string xAxisField, string yAxisField, string[] yAxisInfoFields, string[] yAxisInfoFieldsNames, string zAxisField, string mainColumnName, string columnTotalName, string rowTotalName)
        {
            //style table
            /*
             * The x-axis is the main horizontal row.
             * The z-axis is the sub horizontal row.
             * The y-axis is the left vertical column.
             */

            //get distinct xAxisFields
            ArrayList xAxis = new ArrayList();

            _DataTable.MoveFirst();
            while (!_DataTable.EOF) {
                if (!xAxis.Contains(_DataTable.GetValue(xAxisField))){
                    xAxis.Add(_DataTable.GetValue(xAxisField));
                }
                _DataTable.MoveNext();
            }

            //get distinct yAxisFields
            ArrayList yAxis = new ArrayList();

            _DataTable.MoveFirst();
            while (!_DataTable.EOF) {
                if (!yAxis.Contains(_DataTable.GetValue(yAxisField))){
                    yAxis.Add(_DataTable.GetValue(yAxisField));
                }
                _DataTable.MoveNext();
            }

            //create a 2D array for the x-axis/y-axis fields
            string[,] matrix = new string[xAxis.Count, yAxis.Count];

            for (int y = 0; y < yAxis.Count; y++) { //loop thru y-axis fields
                //rows
                for (int x = 0; x < xAxis.Count; x++) { //loop thru x-axis fields
                    //main columns
                    //get the z-axis values
                    string zAxisValue = FindValue(xAxisField, Convert.ToString(xAxis[x])
                                                  , yAxisField, Convert.ToString(yAxis[y]), zAxisField);
                    matrix[x, y] = zAxisValue;
                }
            }

            //calculate totals for the y-axis
            decimal[] yTotals = new decimal[xAxis.Count];

            for (int col = 0; col < xAxis.Count; col++) {
                yTotals[col] = 0;

                for (int row = 0; row < yAxis.Count; row++) {
                    yTotals[col] += Convert.ToDecimal("0" + matrix[col, row]);
                }
            }

            //calculate totals for the x-axis
            decimal[] xTotals = new decimal[(yAxis.Count + 1)];

            for (int row = 0; row < yAxis.Count; row++) {
                xTotals[row] = 0;

                for (int col = 0; col < xAxis.Count; col++) {
                    xTotals[row] += Convert.ToDecimal("0" + matrix[col, row]);
                }
            }

            xTotals[xTotals.GetUpperBound(0)] = 0; //Grand Total

            for (int i = 0; i < xTotals.GetUpperBound(0); i++) {
                xTotals[xTotals.GetUpperBound(0)] += xTotals[i];
            }

            //Build HTML Table

            //Build HTML Table
            //Append main row (x-axis)
            JSONTable table = new JSONTable();

            AttributeMapping columnYTitle = new AttributeMapping();
            columnYTitle.Name             = mainColumnName;
            columnYTitle.Width            = 8;
            columnYTitle.Caption          = "";

            foreach (string yAxisInfoFieldsName in yAxisInfoFieldsNames) {

                AttributeMapping column = new AttributeMapping();
                column.Name             = yAxisInfoFieldsName ;
                column.Width            = 8;
                column.DataType         = "decimal";
                column.TypeChar         = "n";
                column.Scale            = 0;
                column.Caption          = yAxisInfoFieldsName  ;
                table.Declare(column);
            }

            table.Declare(columnYTitle);

            for (int x = 0; x <= xAxis.Count; x++) { //loop thru x-axis + 1
                if (x < xAxis.Count) {

                    AttributeMapping column = new AttributeMapping();
                    column.Name             = Convert.ToString(xAxis[x]);
                    column.Width            = 8;
                    column.DataType         = "decimal";
                    column.TypeChar         = "n";
                    column.Scale            = 0;
                    column.Caption          = Convert.ToString(xAxis[x]);
                    table.Declare(column);
                } else {
                    if (columnTotalName!=""){
                        AttributeMapping column = new AttributeMapping();
                        column.Name             = columnTotalName;
                        column.Width            = 8;
                        column.DataType         = "decimal";
                        column.TypeChar         = "n";
                        column.Scale            = 0;
                        column.Caption          = columnTotalName;
                        table.Declare(column);
                    }
                }
            }

            //Append table items from matrix
            for (int y = 0; y < yAxis.Count; y++) { //loop thru y-axis
                table.AddNew();

                for (int z = 0; z <= xAxis.Count + yAxisInfoFieldsNames.Length; z++) { //loop thru z-axis + 1
                    if (z < yAxisInfoFieldsNames.Length) {
                        table.SetValue(z , Convert.ToString(_DataTable.GetValue(y , yAxisInfoFields[z])));
                    }

                    if (z == yAxisInfoFieldsNames.Length) {
                        table.SetValue(z , Convert.ToString(yAxis[y]));
                    }

                    if (z > yAxisInfoFieldsNames.Length) {
                        table.SetValue(z , Convert.ToString(matrix[(z - 1 - yAxisInfoFieldsNames.Length) , y]));
                    }
                }

                if (columnTotalName!=""){
                    table.SetValue(xAxis.Count + yAxisInfoFieldsNames.Length  + 1 , Convert.ToString(xTotals[y]));
                }

                table.Update();
            }

            if (rowTotalName!=""){
                //append y-axis totals
                table.AddNew();

                for (int x = 0; x <= (xAxis.Count + 1) + yAxisInfoFieldsNames.Length; x++) {
                    if (x == 0) {
                        table.SetValue(0 , rowTotalName);
                    }

                    if (x > yAxisInfoFieldsNames.Length) {
                        if (x <= xAxis.Count + yAxisInfoFieldsNames.Length)
                            table.SetValue(x , Convert.ToString(yTotals[(x - 1 - yAxisInfoFieldsNames.Length)]));
                        else
                            table.SetValue(x , Convert.ToString(xTotals[xTotals.GetUpperBound(0)]));
                    }
                }

                table.Update();
            }

            return table;
        }

    }
}
