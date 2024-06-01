using Microsoft.AspNetCore.Mvc;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using JaosLib.Models.JaoTables;

namespace JaosLib.Services.JaoTables
{
    public class JaoTableExcelServices : IJaoTableExcelServices
    {
        public const string fileStyle = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public string fileName { get; set; } = "";


        public ExcelPOILib excelPOILib { get; set; } = new ExcelPOILib();
        JaoTable jaoTable = new JaoTable();
        private readonly IJaoTableServices jts;

        public JaoTableExcelServices(IJaoTableServices jaoTableServices)
        {
            jts = jaoTableServices;
        }

        /// <summary>
        /// Create an Excel file from JaosTable and download it.
        /// </summary>
        /// <param name="jaoTable">The structure containing the information to create the table</param>
        /// <param name="fileName">The default file name to download</param>
        /// <param name="sheetName">The name for the sheet where data will be included.</param>
        /// <param name="response">use Respnse or HttpResponse</param>
        /// <param name="controllerBase">Use ControllerBase</param>
        /// <returns></returns>
        public MemoryStream createExcelFile(JaoTable jaoTable, string fileNameRoot, string sheetName, HttpResponse response)
        {
            this.jaoTable = jaoTable;
            excelPOILib = new ExcelPOILib();
            excelPOILib.createSheet(sheetName);

            if (jaoTable.excelWidths?.Length > 0)
                setColumnsWidth(jaoTable.excelWidths);
            else if (jaoTable.rows != null && jaoTable.rows.Any())
                setColumnsWidth(jaoTable.rows[0]);

            displayReportTitle();

            //---------------------------------------------------------------------
            // Not needed for production, just to see the diffetent Styles available
            // Include all available styles in a file to identify Styles 
            //=====================================================================
            //IRow row = excelPOILib.nextRow();
            //excelPOILib.nextCell(row, "SheetHeader", excelPOILib.styleSheetHeader);
            //row = excelPOILib.nextRow();
            //excelPOILib.nextCell(row, "SheetSubHeader", excelPOILib.styleSheetSubheader);
            //row = excelPOILib.nextRow();
            //excelPOILib.nextCell(row, "SheetTableHeader", excelPOILib.styleTableHeader);
            //row = excelPOILib.nextRow();
            //excelPOILib.nextCell(row, "Estandard", excelPOILib.styleText[(int)ExcelPOILib.CellClass.standard]);
            //row = excelPOILib.nextRow();
            //excelPOILib.nextCell(row, "Total", excelPOILib.styleText[(int)ExcelPOILib.CellClass.total]);
            //row = excelPOILib.nextRow();
            //excelPOILib.nextCell(row, "Group", excelPOILib.styleText[(int)ExcelPOILib.CellClass.group]);
            //row = excelPOILib.nextRow();
            //excelPOILib.nextCell(row, "Title", excelPOILib.styleText[(int)ExcelPOILib.CellClass.title]);

            //// Sample all available Colors
            //for (short c = 0; c < 82; c++)
            //{
            //    ICellStyle styleTotal = excelPOILib.CreateStyle(excelPOILib.contentFontBold, HorizontalAlignment.Right, c);
            //    row = excelPOILib.nextRow();
            //    excelPOILib.nextCell(row, "color " + c, styleTotal);
            //}


            createTitleRows();
            createContentRows();

            prepareDownload(fileNameRoot, response);

            MemoryStream ms = new MemoryStream();
            excelPOILib.workbook.Write(ms, true);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        void prepareDownload(string fileNameRoot, HttpResponse response)
        {
            fileName = string.Format($"{fileNameRoot} {DateTime.Now.ToString("yyyy-MM-dd hh:mm")}.xlsx");
            response.Clear();
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.Headers.Add("Content-Disposition", "attachment; filename=\"" + fileName + "\"");

        }


        void setColumnsWidth(JaoRow row)
        {
            // Column widths
            int col = 0;
            foreach (JaoCell cell in row.cells)
            {
                if (cell.cssClass?.Contains(JaoTable.textNormal) ?? false)
                    excelPOILib.setWidth(col, JaoTable.widthNormal);
                else if (cell.cssClass?.Contains(JaoTable.textLong) ?? false)
                    excelPOILib.setWidth(col, JaoTable.widthLong);
                else
                    excelPOILib.setWidth(col, JaoTable.widthShort);
                col++;
            }
        }

        void setColumnsWidth(float[] widths)
        {
            for (int col = 0; col < widths.Length; col++)
            {
                excelPOILib.setWidth(col, widths[col]);// 
            }
        }


        void displayReportTitle()
        {
            if (!string.IsNullOrEmpty(jaoTable.reportTitle))
            {
                IRow row = excelPOILib.nextRow();
                row.Height = 340;
                excelPOILib.nextCell(row, jaoTable.reportTitle, excelPOILib.styleSheetHeader);
                if (jts.numberOfColumns() > 1)
                    excelPOILib.expandCell(1, jts.numberOfColumns());
            }
            if (!string.IsNullOrEmpty(jaoTable.reportSubtitle))
            {
                IRow row = excelPOILib.nextRow();
                row.Height = (short)(row.Height * 1.3);
                excelPOILib.nextCell(row, jaoTable.reportSubtitle, excelPOILib.styleSheetSubheader);
                if (jts.numberOfColumns() > 1)
                    excelPOILib.expandCell(1, jts.numberOfColumns());
            }
        }

        void createTitleRows()
        {
            if (jaoTable.headerRows != null && jaoTable.headerRows.Any())
                foreach (JaoRow titleRow in jaoTable.headerRows)
                    createTitleRow(titleRow);
        }
        void createTitleRow(JaoRow titleRow)
        {
            IRow row = excelPOILib.nextRow();
            if (titleRow != null && titleRow.cells != null && titleRow.cells.Any())
                foreach (JaoCell cell in titleRow.cells)
                {
                    excelPOILib.nextCell(row, cell.text, excelPOILib.styleTableHeader);
                    if (cell.span > 0)
                        excelPOILib.expandCell(1, cell.span);
                }
        }


        void createContentRows()
        {
            if (jaoTable.rows != null && jaoTable.rows.Any())
                foreach (JaoRow row in jaoTable.rows)
                    createRow(row);
        }
        void createRow(JaoRow row)
        {
            IRow excelRow = excelPOILib.nextRow();
            foreach (JaoCell cell in row.cells)
            {
                switch (cell.contentType)
                {
                    case JaoTable.ContentType.int_:
                        excelPOILib.nextCell(excelRow, cell.intNumber, getCellStyle(cell, row));
                        break;
                    case JaoTable.ContentType.float_:
                        excelPOILib.nextCell(excelRow, cell.doubleNumber, getCellStyle(cell, row));
                        break;
                    case JaoTable.ContentType.double_:
                        excelPOILib.nextCell(excelRow, cell.doubleNumber, getCellStyle(cell, row));
                        break;
                    case JaoTable.ContentType.date_:
                        excelPOILib.nextDateCell(excelRow, cell.date, getCellStyle(cell, row));
                        break;
                    case JaoTable.ContentType.bool_:
                        excelPOILib.nextBoolCell(excelRow, cell.boolean, getCellStyle(cell, row));
                        break;
                    default:
                        excelPOILib.nextCell(excelRow, cell.text, getCellStyle(cell, row));
                        break;
                }
                if (cell.span > 0)
                    excelPOILib.expandCell(1, cell.span);
            }
        }

        ICellStyle getCellStyle(JaoCell cell, JaoRow row)
        {
            ExcelPOILib.CellClass cellClass;

            if (row.cssClass != null && row.cssClass.Contains("title"))
                return excelPOILib.styleTableHeader;

            string cssClass = cell.cssClass + row.cssClass;
            if (cssClass.Contains(JaoTable.emptyClass))
                return excelPOILib.styleEmpty;
            if (cssClass.Contains(JaoTable.groupClass))
                cellClass = ExcelPOILib.CellClass.group;
            else if (cssClass.Contains(JaoTable.totalsClass))
                cellClass = ExcelPOILib.CellClass.total;
            else if (cssClass.Contains(JaoTable.titleClass))
                cellClass = ExcelPOILib.CellClass.title;
            else
                cellClass = ExcelPOILib.CellClass.standard;

            switch (cell.contentType)
            {
                case JaoTable.ContentType.date_:
                    return excelPOILib.styleDate[(int)cellClass];
                case JaoTable.ContentType.int_:
                case JaoTable.ContentType.double_:
                    return excelPOILib.styleInt[(int)cellClass];
                case JaoTable.ContentType.float_:
                    return excelPOILib.styleDecimal[(int)cellClass];
                default:
                    return excelPOILib.styleText[(int)cellClass];
            }
        }


        /// </summary>
        /// <param name="fontColor">Font color (see HSSFColor)</param>
        /// <param name="fontHeight">Font height in points</param>
        /// <param name="fontBold">true or false</param>
        /// <param name="fontName"></param>
        /// <returns>new IFont</returns>
        private IFont CreateFont(short fontColor, short fontHeight, bool fontBold, string fontName = ExcelPOILib.fontCalibri)
        {

            IFont font = excelPOILib.workbook.CreateFont();
            font.IsBold = fontBold;
            font.Color = fontColor;
            font.FontName = fontName;
            font.FontHeightInPoints = fontHeight;

            return font;
        }
    }
}
