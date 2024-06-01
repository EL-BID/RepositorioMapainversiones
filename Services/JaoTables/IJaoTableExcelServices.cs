
using Microsoft.AspNetCore.Mvc;
using JaosLib.Models.JaoTables;

namespace JaosLib.Services.JaoTables
{
    public interface IJaoTableExcelServices
    {
        string fileName { get; set; }

        MemoryStream createExcelFile(JaoTable jaoTable, string fileNameRoot, string sheetName, HttpResponse response);
    }

}
