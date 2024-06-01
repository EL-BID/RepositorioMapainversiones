#nullable disable

using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using IMRepo;
using static JaosLib.Services.JaoTables.ExcelPOILib;

namespace JaosLib.Services.JaoTables
{
    public class ExcelPOILib : IExcelPOILib
    {
        public enum CellClass { standard, total, group, title }

        // Excel work book
        public IWorkbook workbook;
        public ISheet sheet;

        // Fonts
        public IFont sheetHeaderFont;
        public IFont sheetSubheaderFont;
        public IFont tableHeaderFont;
        public IFont smallerHeaderFont;
        public IFont contentFont;
        public IFont contentFontBold;
        public IFont smallerContentFont;

        // Styles
        public ICellStyle styleSheetHeader;
        public ICellStyle styleSheetSubheader;
        public ICellStyle styleTableHeader;
        public ICellStyle styleEmpty;

        public ICellStyle[] styleText = new ICellStyle[4];
        public ICellStyle[] styleInt = new ICellStyle[4];
        public ICellStyle[] styleDecimal = new ICellStyle[4];
        public ICellStyle[] styleDate = new ICellStyle[4];

        // Integer to store the index of the next row
        public int rowIndex;
        public int colIndex;


        public const string fontCalibri = "Calibri";
        public const string fontArialNarrow = "Arial Narrow";
        public const string fonBookAntigua = "Book Antiqua";




        public ExcelPOILib()
        {
            // Initialize rowIndex
            rowIndex = 0;
            colIndex = 0;

            // New Workbook
            workbook = new XSSFWorkbook();

            generateFonts();
            generateStyles();
        }


        /// <summary>
        /// Create a new sheet in the workbook and set it as the active sheet.
        /// </summary>
        /// <param name="name">The name for the new sheet</param>
        /// <returns>The new sheet</returns>
        public ISheet createSheet(string name)
        {
            return sheet = workbook.CreateSheet(name);
        }


        //*************************************************************************************************/
        //
        //                      Generic Excel Methods
        //
        //-------------------------------------------------------------------------------------------------

        //==============================================
        //                             Cells
        //----------------------------------------------

        public ICell createCell(IRow row, int colNumber, string content, ICellStyle style)
        {
            colIndex = colNumber + 1;
            ICell cell = row.CreateCell(colNumber);
            cell.CellStyle = style;
            cell.SetCellValue(content);
            return cell;
        }
        public ICell createNumericCell(IRow row, int colNumber, string content, ICellStyle style)
        {
            ICell cell = createCell(row, colNumber, content, style);
            cell.SetCellType(CellType.Numeric);
            return cell;
        }


        public ICell nextCell(IRow row, string content, ICellStyle style)
        {
            ICell cell = row.CreateCell(colIndex++);
            cell.CellStyle = style;
            if (content != null)
            {
                if (content.Length < ProjectGlobals.MaxCellStringLength)
                    cell.SetCellValue(content);
                else
                    cell.SetCellValue(content.Substring(0, ProjectGlobals.MaxCellStringLength));
            }
            return cell;
        }

        public ICell nextCell(IRow row, double? content, ICellStyle style)
        {
            ICell cell = row.CreateCell(colIndex++);

            cell.CellStyle = style;
            cell.SetCellType(CellType.Numeric);
            if (content.HasValue)
            {
                cell.SetCellValue(content.Value);
            }
            else
                cell.SetCellValue("");
            return cell;
        }

        public ICell nextNumericCell(IRow row, string content, ICellStyle style)
        {
            ICell cell = row.CreateCell(colIndex++);
            cell.CellStyle = style;
            cell.SetCellType(CellType.Numeric);
            if (content != "")
                cell.SetCellValue(Convert.ToDouble(content));
            else
                cell.SetCellValue("");
            return cell;
        }

        public ICell nextDateCell(IRow row, DateTime? date, ICellStyle style)
        {
            ICell cell = row.CreateCell(colIndex++);
            cell.CellStyle = style;
            cell.SetCellType(CellType.Numeric);
            if (date.HasValue && date.Value != DateTime.MinValue)
                cell.SetCellValue(Convert.ToDateTime(date));
            else
                cell.SetCellValue("");
            return cell;
        }

        public ICell nextBoolCell(IRow row, bool? content, ICellStyle style)
        {
            ICell cell = row.CreateCell(colIndex++);
            cell.CellStyle = style;
            cell.SetCellType(CellType.Boolean);
            if (content.HasValue)
                cell.SetCellValue(content.Value ? "TRUE" : "FALSE");
            else
                cell.SetCellValue("");
            return cell;
        }


        //==============================================
        //                             Rows
        //----------------------------------------------

        public IRow nextRow()
        {
            colIndex = 0;
            return sheet.CreateRow(rowIndex++);
        }

        public IRow nextRow(ISheet sheet)
        {
            colIndex = 0;
            return sheet.CreateRow(rowIndex++);
        }

        public void setColumnsWidth(ISheet sheet, List<double> colwidths)
        {
            for (int i = 0; i < colwidths.Count; i++)
                setWidth(sheet, i, colwidths[i]);

        }

        //==============================================
        //                        Fonts and Styles 
        //----------------------------------------------


        /**
         * Create a new font on base workbook
         * 
         * @param fontColor       Font color (see {@link HSSFColor})
         * @param fontHeight      Font height in points
         * @param fontBold        Font is boldweight (<code>true</code>) or not (<code>false</code>)
         * 
         * @return New cell style
         */
        //public HSSFFont createFont(short fontColor, short fontHeight, Boolean fontBold)
        public IFont CreateFont(short fontColor, short fontHeight, bool fontBold, string fontName = fontCalibri)
        {

            IFont font = workbook.CreateFont();
            font.IsBold = fontBold;
            font.Color = fontColor;
            font.FontName = fontName;
            font.FontHeightInPoints = fontHeight;

            return font;
        }


        /// <summary>
        /// Create a style on base workbook
        /// </summary>
        /// <param name="font">Font used by the style</param>
        /// <param name="cellAlign">Cell alignment for contained text (HSSFCellStyle)</param>
        /// <param name="backgroundColor">Cell background color (see {@link HSSFColor})</param>
        /// <returns></returns>
        public ICellStyle CreateStyle(IFont font, HorizontalAlignment cellAlign, short backgroundColor)
        {
            ICellStyle style = workbook.CreateCellStyle();
            style.SetFont(font);
            style.Alignment = cellAlign;  // (cellAlign);
            style.WrapText = true;
            if (backgroundColor != HSSFColor.COLOR_NORMAL)
            {
                style.FillForegroundColor = backgroundColor;
                style.FillPattern = FillPattern.SolidForeground;
            }

            return style;
        }

        /// <summary>
        /// Sets a border for a Style
        /// </summary>
        /// <param name="style">The style for which the border will be set.</param>
        /// <param name="top">top border (BorderStyle.Thin, None, Medium or Thick)</param>
        /// <param name="right">right border style</param>
        /// <param name="bottom">bottom border style</param>
        /// <param name="left">left border style</param>
        /// <param name="color">The color for all the borders of the style</param>
        public void borderFormat(ICellStyle style, BorderStyle top, BorderStyle right, BorderStyle bottom, BorderStyle left, short color)
        {
            if (top > 0)
            {
                style.BorderTop = top;
                style.TopBorderColor = color;
            }
            if (right > 0)
            {
                style.BorderRight = right;
                style.RightBorderColor = color;
            }
            if (bottom > 0)
            {
                style.BorderBottom = bottom;
                style.BottomBorderColor = color;
            }
            if (left > 0)
            {
                style.BorderLeft = left;
                style.LeftBorderColor = color;
            }
        }



        const int factor = 66;
        public void setWidth(int col, double? ancho)
        {
            sheet.SetColumnWidth(col, Convert.ToInt32((ancho.HasValue ? ancho.Value : 1) * factor));
        }

        public void setWidth(ISheet sheet, int col, double ancho)
        {
            sheet.SetColumnWidth(col, Convert.ToInt32(ancho * factor));
        }

        public void expandCell(ISheet sheet, int rowNumber, int colNumber, int height, int width)
        {
            rowNumber--;
            int endRow = rowNumber + height;
            colIndex = colNumber + width;
            sheet.AddMergedRegion(new CellRangeAddress(rowNumber, endRow - 1, colNumber, colIndex - 1));
        }
        public void expandCell(ISheet sheet, int height, int width)
        {
            int rowNumber = rowIndex - 1;
            int colNumber = colIndex - 1;
            int endRow = rowNumber + height;
            colIndex = colNumber + width;
            sheet.AddMergedRegion(new CellRangeAddress(rowNumber, endRow - 1, colNumber, colIndex - 1));
        }
        public void expandCell(int height, int? width)
        {
            int rowNumber = rowIndex - 1;
            int colNumber = colIndex - 1;
            int endRow = rowNumber + height;
            colIndex = colNumber + (width.HasValue ? width.Value : 1);
            sheet.AddMergedRegion(new CellRangeAddress(rowNumber, endRow - 1, colNumber, colIndex - 1));
        }




        public void generateFonts()
        {
            // Generate fonts
            sheetHeaderFont = CreateFont(HSSFColor.Black.Index, 12, true, fontCalibri);
            sheetSubheaderFont = CreateFont(HSSFColor.Black.Index, 11, false, fontCalibri);
            tableHeaderFont = CreateFont(HSSFColor.Black.Index, 11, true, fontCalibri);
            smallerHeaderFont = CreateFont(HSSFColor.Black.Index, 10, true, fontCalibri);
            contentFont = CreateFont(HSSFColor.Black.Index, 11, false);
            contentFontBold = CreateFont(HSSFColor.Black.Index, 11, true);
            smallerContentFont = CreateFont(HSSFColor.Black.Index, 10, false);
        }

        short colorGrayLight = 73; // NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
        short colorGrayMedium = 9;// NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;

        public void generateStyles()
        {

            //            Generate styles
            //HSSFPalette palette = workbook.pale.GetCustomPalette();
            //HSSFColor colorData = palette.FindSimilarColor(146, 208, 80);
            //HSSFColor colorStrong = palette.FindSimilarColor(255, 255, 0);
            //HSSFColor colorLight = palette.FindSimilarColor(220, 220, 220);

            //HSSFColor.Grey25Percent.Index
            styleSheetHeader = CreateStyle(sheetHeaderFont, HorizontalAlignment.Center, HSSFColor.COLOR_NORMAL);
            styleSheetHeader.VerticalAlignment = VerticalAlignment.Top;
            styleSheetSubheader = CreateStyle(sheetSubheaderFont, HorizontalAlignment.Center, HSSFColor.COLOR_NORMAL);
            styleSheetSubheader.VerticalAlignment = VerticalAlignment.Top;

            styleTableHeader = CreateStyle(tableHeaderFont, HorizontalAlignment.Center, colorGrayMedium);
            styleTableHeader.VerticalAlignment = VerticalAlignment.Center;
            borderFormat(styleTableHeader, BorderStyle.Medium, 0, BorderStyle.Medium, 0, HSSFColor.Black.Index);

            styleEmpty = CreateStyle(contentFont, HorizontalAlignment.Center, HSSFColor.COLOR_NORMAL);
            borderFormat(styleEmpty, BorderStyle.None, BorderStyle.None, BorderStyle.None, BorderStyle.None, HSSFColor.COLOR_NORMAL);

            // Data type styles (standard, ,total ,group ,title)
            for (int cellClass = 0; cellClass < Enum.GetNames(typeof(CellClass)).Length; cellClass++)
            {
                styleText[cellClass] = getBasicStyle(cellClass, HorizontalAlignment.Left, "");

                styleInt[cellClass] = getBasicStyle(cellClass, HorizontalAlignment.Right, "#,##0");

                styleDecimal[cellClass] = getBasicStyle(cellClass, HorizontalAlignment.Right, "#,##0.00");

                styleDate[cellClass] = getBasicStyle(cellClass, HorizontalAlignment.Center, "dd/MMM/yyyy");
            }
        }

        ICellStyle getBasicStyle(int cellClass, HorizontalAlignment alignment, string dataFormat)
        {
            ICellStyle style;
            switch (cellClass)
            {
                case (int)CellClass.total:
                    style = CreateStyle(contentFontBold, alignment, HSSFColor.COLOR_NORMAL);
                    borderFormat(style, BorderStyle.Medium, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, HSSFColor.Grey50Percent.Index);
                    break;
                case (int)CellClass.title:
                    style = CreateStyle(contentFontBold, alignment, HSSFColor.COLOR_NORMAL);
                    borderFormat(style, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, HSSFColor.Grey50Percent.Index);
                    break;
                case (int)CellClass.group:
                    style = CreateStyle(contentFont, alignment, colorGrayLight);
                    borderFormat(style, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, HSSFColor.Grey50Percent.Index);
                    break;
                default:
                    style = CreateStyle(contentFont, alignment, HSSFColor.COLOR_NORMAL);
                    borderFormat(style, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, BorderStyle.Thin, HSSFColor.Grey50Percent.Index);
                    break;
            }
            if (!string.IsNullOrEmpty(dataFormat))
                style.DataFormat = workbook.CreateDataFormat().GetFormat(dataFormat);
            return style;
        }


    }
}
