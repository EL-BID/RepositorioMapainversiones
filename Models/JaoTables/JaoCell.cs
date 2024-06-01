using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Net.Mime;
using NPOI.SS.UserModel;

namespace JaosLib.Models.JaoTables
{
    public class JaoCell
    {
        [DisplayName("Name")]

        public string text { get; set; } = string.Empty;
        public double? doubleNumber { get; set; }
        public int? intNumber { get; set; }
        public DateTime? date { get; set; }
        public bool? boolean { get; set; }

        public string? html { get; set; }
        public string? cssClass { get; set; } = "";
        public int? span { get; set; }
        public bool? beginSection { get; set; }
        public JaoTable.ContentType contentType { get; set; } = JaoTable.ContentType.text_;

        public string toString()
        {
            switch (contentType)
            {
                case JaoTable.ContentType.int_:
                    return intNumber.HasValue ? intNumber.Value.ToString("#,##0") : "";
                case JaoTable.ContentType.float_:
                    return doubleNumber.HasValue ? doubleNumber.Value.ToString("#,##0.00") : "";
                case JaoTable.ContentType.double_:
                    return doubleNumber.HasValue ? doubleNumber.Value.ToString("#,##0") : "";
                case JaoTable.ContentType.date_:
                    return date.HasValue && date.Value != DateTime.MinValue ? date.Value.ToString("dd/MMM/yyyy") : "";
                case JaoTable.ContentType.bool_:
                    return boolean.HasValue ? boolean.Value.ToString() : "";
                default:
                    return text ?? "";
            }
        }

    }
}
