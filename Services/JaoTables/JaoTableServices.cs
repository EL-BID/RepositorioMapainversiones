using NPOI.SS.Formula.Functions;
using JaosLib.Models.JaoTables;
using NuGet.Protocol;

namespace JaosLib.Services.JaoTables
{
    public class JaoTableServices : IJaoTableServices
    {
        JaoTable table = new JaoTable();
        JaoRow? currentHeaderRow;
        JaoRow? currentRow;
        JaoCell currentCell = new JaoCell();
        JaoCell currentHeaderCell = new JaoCell();

        public JaoTable getTable()
        {
            return table;
        }

        public void setTitle(string title)
        {
            table.reportTitle = title;
        }
        public void setSubtitle(string subtitle)
        {
            table.reportSubtitle = subtitle;
        }

        public void setExcelWidths(float[] widths)
        {
            table.excelWidths = widths;

        }

        public void addHeaderRow()
        {
            currentHeaderRow = new JaoRow();
            table.headerRows?.Add(currentHeaderRow);
        }
        public void addHeaderCell()
        {
            if (currentHeaderRow == null)
                addHeaderRow();
            currentHeaderCell = new JaoCell();
            currentHeaderRow?.cells?.Add(currentHeaderCell);
        }
        public void addHeaderCell(string text, string cssClass = "")
        {
            addHeaderCell();
            currentHeaderCell.contentType = JaoTable.ContentType.text_;
            currentHeaderCell.text = text;
            if (!string.IsNullOrEmpty(cssClass))
                currentHeaderCell.cssClass = cssClass;
        }
        public void addHeaderCell(int? number)
        {
            addHeaderCell();
            currentHeaderCell.contentType = JaoTable.ContentType.int_;
            currentHeaderCell.intNumber = number;
        }
        public void addHeaderCell(double? number)
        {
            addHeaderCell();
            currentHeaderCell.contentType = JaoTable.ContentType.double_;
            currentHeaderCell.doubleNumber = number;
        }
        public void addHeaderCell(DateTime? date)
        {
            addHeaderCell();
            currentHeaderCell.contentType = JaoTable.ContentType.date_;
            currentHeaderCell.date = date;
        }
        public void addHeaderCell(bool? boolean)
        {
            addHeaderCell();
            currentHeaderCell.contentType = JaoTable.ContentType.bool_;
            currentHeaderCell.boolean = boolean;
        }
        public void addRow()
        {
            currentRow = new JaoRow();
            table.rows.Add(currentRow);
        }
        public void addCell()
        {
            if (currentRow == null)
                addRow();
            currentCell = new JaoCell();
            currentRow!.cells.Add(currentCell);
        }
        public void addCell(string? text, string? cssClass = null)
        {
            addCell();
            currentCell.contentType = JaoTable.ContentType.text_;
            if (cssClass != null)
                currentCell.cssClass = cssClass;
            if (text != null)
                currentCell.text = text;
        }
        public void addCell(int? number, string? cssClass = null)
        {
            addCell();
            currentCell.contentType = JaoTable.ContentType.int_;
            currentCell.cssClass = setClass(JaoTable.classNumber, cssClass);
            currentCell.intNumber = number;
        }
        public void addFloatCell(float? number, string? cssClass = null)
        {
            addCell();
            currentCell.contentType = JaoTable.ContentType.float_;
            currentCell.cssClass = setClass(JaoTable.classNumber, cssClass);
            currentCell.doubleNumber = number;
        }
        public void addCell(double? number, string? cssClass = null)
        {
            addCell();
            currentCell.contentType = JaoTable.ContentType.double_;
            currentCell.cssClass = setClass(JaoTable.classNumber, cssClass);
            currentCell.doubleNumber = number;
        }
        public void addCell(DateTime? date, string? cssClass = null)
        {
            addCell();
            currentCell.contentType = JaoTable.ContentType.date_;
            currentCell.cssClass = setClass(JaoTable.classDate, cssClass);
            currentCell.date = date;
        }
        public void addCell(bool? boolean, string? cssClass = null)
        {
            addCell();
            currentCell.contentType = JaoTable.ContentType.bool_;
            if (cssClass != null)
                currentCell.cssClass = cssClass;
            currentCell.boolean = boolean;
        }

        public string setClass(params string?[] classes)
        {
            return string.Join("*", classes.Where(c => !string.IsNullOrEmpty(c)));
        }

        public int numberOfColumns()
        {
            if (table.excelWidths.Length > 0)
                return table.excelWidths.Length;
            if (table.headerRows != null)
                if (table.headerRows.Count > 0)
                    return numberOfColumns(table.headerRows[0]);
            if (table.rows.Count > 0)
                return table.rows[0].cells.Count;
            return 0;
        }

        public int numberOfColumns(JaoRow row)
        {
            if (row != null && row.cells != null && row.cells.Any())
            {
                int? columns = row.cells.Sum(c => c.span > 0 ? c.span : 1);
                if (columns.HasValue)
                    return columns.Value;
            }
            return 0;
        }
    }
}
