
using JaosLib.Models.JaoTables;

namespace JaosLib.Services.JaoTables
{
    public interface IJaoTableServices
    {
        JaoTable getTable();
        void setTitle(string title);
        void setSubtitle(string subtitle);
        void setExcelWidths(float[] widths);
        void addHeaderRow();
        void addHeaderCell();
        void addHeaderCell(string text, string cssClass = "");
        void addHeaderCell(int? number);
        void addHeaderCell(double? number);
        void addHeaderCell(DateTime? date);
        void addHeaderCell(bool? boolean);
        void addRow();
        void addCell();
        void addCell(string? text, string? cssClass = null);
        void addCell(int? number, string? cssClass = null);
        void addFloatCell(float? number, string? cssClass = null);
        void addCell(double? number, string? cssClass = null);
        void addCell(DateTime? date, string? cssClass = null);
        void addCell(bool? boolean, string? cssClass = null);
        int numberOfColumns();
        int numberOfColumns(JaoRow row);

    }

}
