using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula;
using NPOI.XWPF.UserModel;
using IMRepo.Models.Domain;
using static NPOI.XWPF.UserModel.XWPFTable;

namespace JaosLib.Services.Utilities
{
    public interface INPOIWordService
    {

        #region Word Utilities

        //==================================================================
        //
        //                    NPOI Word Utilities
        //
        //------------------------------------------------------------------


        //======================
        //       Document
        //----------------------

        XWPFDocument getDoc();
        void setDoc(XWPFDocument doc);
        void createWordDocument();
        void saveWordDocument();

        FileStreamResult downloadDocument(string fileName, ControllerBase controllerBase);
        FileStreamResult downloadReport(HttpResponse httpResponse, string fileName, ControllerBase controllerBase);


        //========================
        //       T i t l e s
        //------------------------
        void centeredTitle(string? text, int? fontSize = null);
        XWPFParagraph? title(string title);
        XWPFParagraph titledParagraph(string title, string? text);
        XWPFParagraph headedParagraph(string title, string? text);
        void titleRun(string title, XWPFParagraph paragraph);
        void resetChapterNumber(int? number = null);
        XWPFParagraph? numberedTitle(string title);
        XWPFParagraph numberedTitledParagraph(string title, string? text);
        void numberedTitleRun(string title, XWPFParagraph paragraph);


        //======================
        //       Paragraph
        //----------------------
        void emptyLine();
        XWPFParagraph paragraph(string? text);
        void paragraphRun(string? text, XWPFParagraph paragraph);
        void pageBreak();


        #endregion
        #region Tables Utilities

        //===========================================
        //        Tables Utilities
        //-------------------------------------------

        XWPFTable createTable(ulong[] colWidths);
        void setColWidths(XWPFTable table, ulong[] colWidths);

        void setCell(XWPFTableRow row, int col, double? number, bool bold = false);

        void cellTextBold(XWPFTableCell cell, string text, ParagraphAlignment alignment);
        void cellTextBold(XWPFTableCell cell, string text1, string text2, string text3, ParagraphAlignment alignment);
        void cellTextBold(XWPFTableCell cell, string[] texts, ParagraphAlignment alignment);
        void cellText(XWPFTableCell cell, string? text, ParagraphAlignment alignment);
        void cellText(XWPFTableCell cell, string text1, string text2, string text3, ParagraphAlignment alignment);
        XWPFRun? cellRun(string title, XWPFParagraph paragraph, ParagraphAlignment alignment, bool bold);
        #endregion
        #region Other Utilities

        //================================================================
        //
        //           O t h e r   L i b s
        //
        //----------------------------------------------------------------
        double? add(double? number1, double? number2);
        string display(DateTime? date);
        string displayMonth(DateTime? date);

        #endregion
    }
}
