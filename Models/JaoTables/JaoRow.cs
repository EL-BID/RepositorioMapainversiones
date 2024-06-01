using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Net.Mime;

namespace JaosLib.Models.JaoTables
{
    public class JaoRow
    {
        public List<JaoCell> cells { get; set; } = new List<JaoCell>(); 
        public string? cssClass { get; set; }
    }
}
