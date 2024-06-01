using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Net.Mime;

namespace JaosLib.Models.JaoTables
{
    public class JaoTable
    {
        public static string groupClass = "group";
        public static string totalsClass = "total";
        public static string beginSectionClass = "bs";
        public static string defaultClass = "report";
        public static string emptyClass = "empty";
        public static string titleClass = "title";

        public const string textShort = "short";
        public const string textNormal = "normal";
        public const string textLong = "long";

        public const int widthShort = 60;
        public const int widthNormal = 140;
        public const int widthLong = 240;

        public enum ContentType { text_ = 0, bool_ = 1, date_ = 2, int_ = 3, double_ = 4, float_ = 5 }

        public static string screenOnlyClass = "screenOnly";
        public static string padAndScreenClass = "padAndScreen";

        public static string[] textClasses = { textShort, textNormal, textLong };
        public static string dateFormat = "dd-MMM-yyyy";

        public static string classNumber = "numCel";
        public static string classDate = "dateCel";


        public string? Name_entityType { get; set; }

        public string? reportTitle { get; set; }
        public string? reportSubtitle { get; set; }
        public List<JaoRow> headerRows { get; set; } = new List<JaoRow>();
        public List<JaoRow> rows { get; set; } = new List<JaoRow>();
        public string? cssClass { get; set; }
        // to be used when the first row won't determine the number of columns and the widths.
        public float[] excelWidths = new float[0];

        //public CssStyleCollection? style;

        public int numberOfColumns()
        {
            if (excelWidths.Length > 0)
                return excelWidths.Length;
            if (headerRows != null)
                if (headerRows.Count > 0)
                    return headerRows[0].cells.Count;
            if (rows.Count > 0)
                return rows[0].cells.Count;
            return 0;
        }

    }
}
