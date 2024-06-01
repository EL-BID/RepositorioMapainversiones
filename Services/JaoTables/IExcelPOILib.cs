
using Microsoft.AspNetCore.Mvc;
using JaosLib.Models.JaoTables;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using static JaosLib.Services.JaoTables.ExcelPOILib;

namespace JaosLib.Services.JaoTables
{
    public interface IExcelPOILib
    {


        /// <summary>
        /// Create a new sheet in the workbook and set it as the active sheet.
        /// </summary>
        /// <param name="name">The name for the new sheet</param>
        /// <returns>The new sheet</returns>
        ISheet createSheet(string name);

        //*************************************************************************************************/
        //
        //                      Generic Excel Methods
        //
        //-------------------------------------------------------------------------------------------------

        //==============================================
        //                             Cells
        //----------------------------------------------

        ICell createCell(IRow row, int colNumber, string content, ICellStyle style);
        ICell createNumericCell(IRow row, int colNumber, string content, ICellStyle style);


        ICell nextCell(IRow row, string content, ICellStyle style);

        ICell nextCell(IRow row, double? content, ICellStyle style);

        ICell nextNumericCell(IRow row, string content, ICellStyle style);

        ICell nextDateCell(IRow row, DateTime? date, ICellStyle style);

        ICell nextBoolCell(IRow row, bool? content, ICellStyle style)
       ;


        //==============================================
        //                             Rows
        //----------------------------------------------

        IRow nextRow();
        IRow nextRow(ISheet sheet);
        void setColumnsWidth(ISheet sheet, List<double> colwidths);

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
        // HSSFFont createFont(short fontColor, short fontHeight, Boolean fontBold)
        IFont CreateFont(short fontColor, short fontHeight, bool fontBold, string fontName = fontCalibri)
       ;


        /// <summary>
        /// Create a style on base workbook
        /// </summary>
        /// <param name="font">Font used by the style</param>
        /// <param name="cellAlign">Cell alignment for contained text (HSSFCellStyle)</param>
        /// <param name="backgroundColor">Cell background color (see {@link HSSFColor})</param>
        /// <returns></returns>
        ICellStyle CreateStyle(IFont font, HorizontalAlignment cellAlign, short backgroundColor)
       ;

        /// <summary>
        /// Sets a border for a Style
        /// </summary>
        /// <param name="style">The style for which the border will be set.</param>
        /// <param name="top">top border (BorderStyle.Thin, None, Medium or Thick)</param>
        /// <param name="right">right border style</param>
        /// <param name="bottom">bottom border style</param>
        /// <param name="left">left border style</param>
        /// <param name="color">The color for all the borders of the style</param>
        void borderFormat(ICellStyle style, BorderStyle top, BorderStyle right, BorderStyle bottom, BorderStyle left, short color)
;

        void setWidth(int col, double? ancho)
       ;

        void setWidth(ISheet sheet, int col, double ancho)
       ;

        void expandCell(ISheet sheet, int rowNumber, int colNumber, int height, int width)
       ;
        void expandCell(ISheet sheet, int height, int width)
; void expandCell(int height, int? width)
;



        void generateFonts()
;

        void generateStyles()
;



    }

}
