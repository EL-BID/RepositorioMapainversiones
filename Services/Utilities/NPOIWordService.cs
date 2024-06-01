using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using static NPOI.XWPF.UserModel.XWPFTable;

namespace JaosLib.Services.Utilities
{
    public class NPOIWordService : INPOIWordService
    {
        public XWPFDocument doc;

        int titleNumber = 1;
        public static int normalFontSize = 11;
        public static int titleFontSize = 12;
        public static int headTitleFontSize = 14;

        public NPOIWordService()
        {
            doc = new XWPFDocument();


            titleNumber = 1;
        }


        #region Word Utilities

        //==================================================================
        //
        //                    NPOI Word Utilities
        //
        //------------------------------------------------------------------

        public XWPFDocument getDoc()
        {
            return doc;
        }

        public void setDoc(XWPFDocument doc)
        {
            this.doc = doc;
        }







        //======================
        //       Document
        //----------------------

        public void createWordDocument()
        {
            doc = new XWPFDocument();

            titleNumber = 1;
        }

        public void saveWordDocument()
        {
            try
            {
                using FileStream sw = System.IO.File.Create($"Issues Report.docx");
                doc.Write(sw);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">From a controller send Request</param>
        /// <param name="controllerBase">From a controller send this as the parameter.</param>
        /// <returns></returns>
        public FileStreamResult downloadDocument(string fileName, ControllerBase controllerBase)
        {
            string path = "wwwroot/TempFiles/";
            fileName += DateTime.Now.ToString("yyyyMMdd") + @".docx";
            string tempFile = path + Path.GetRandomFileName();

            var memoryStream = new MemoryStream();
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                doc.Write(fs);
            }
            using (var fileStream = new FileStream(tempFile, FileMode.Open))
            {
                fileStream.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;
            return controllerBase.File(memoryStream, "application/msword", fileName);

        }



        public FileStreamResult downloadReport(HttpResponse httpResponse, string fileName, ControllerBase controllerBase)
        {
            using (var exportData = new MemoryStream())
            {
                doc.Write(exportData);
                exportData.Close();
                string saveAsFileName = string.Format("{0}-{1}.xls", fileName, DateTime.Now.ToString("yyyyMMdd"));

                return controllerBase.File(exportData, "application/msword", fileName);
            }

        }


        ////PROCESO
        // Quitar los comentarios a los que dicen proceso
        // toman el archivo guardado y le agregan al principio el reporte.
        // El problema actual es la ubicación de las tablas y el estilo con que las crea.
        //
        // ajustar en este archivo y en ReportController de PEU
        //int initialPos = 0;
        //XWPFParagraph IncludeParagraph()
        //{
        //    XWPFParagraph paragraph = doc.CreateParagraph();
        //    var idx = doc.Paragraphs.Count - 1;
        //    for (int i = idx; i > initialPos; i--)
        //    {
        //        doc.SetParagraph(doc.Paragraphs[i - 1], i);
        //    }
        //    doc.SetParagraph(paragraph, initialPos++);
        //    return paragraph;
        //}

        XWPFParagraph IncludeParagraph()
        {
            return doc.CreateParagraph();
        }



        //========================
        //       T i t l e s
        //------------------------
        public void centeredTitle(string? text, int? fontSize = null)
        {
            if (text != null)
            {
                XWPFParagraph paragraph = IncludeParagraph();
                paragraph.Alignment = ParagraphAlignment.CENTER;
                XWPFRun run = paragraph.CreateRun();
                run.FontSize = fontSize ?? headTitleFontSize;
                run.IsBold = true;
                run.SetText(text);
            }
        }

        public XWPFParagraph? title(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                XWPFParagraph paragraph = IncludeParagraph();
                titleRun(title, paragraph);
                return paragraph;
            }
            else
                return null;
        }

        /// <summary>
        /// Displays a Paragraph with a title in bold and a text in normal
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XWPFParagraph titledParagraph(string title, string? text)
        {
            XWPFParagraph paragraph1 = IncludeParagraph();
            paragraph1.Alignment = ParagraphAlignment.LEFT;
            titleRun(title, paragraph1);
            XWPFParagraph paragraph2 = IncludeParagraph();
            paragraph2.Alignment = ParagraphAlignment.LEFT;
            enrichedText(text ?? "", paragraph2);
            return paragraph2;
        }

        /// <summary>
        /// Displays a paragraph with a title in normal and a text in bold
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public XWPFParagraph headedParagraph(string title, string? text)
        {
            XWPFParagraph paragraph = IncludeParagraph();
            paragraph.Alignment = ParagraphAlignment.LEFT;
            paragraphRun(title, paragraph);
            titleRun(text ?? "", paragraph);
            return paragraph;
        }



        public void titleRun(string title, XWPFParagraph paragraph)
        {
            if (!string.IsNullOrEmpty(title))
            {
                XWPFRun run = paragraph.CreateRun();
                run.IsBold = true;
                run.FontSize = titleFontSize;
                run.AppendText(title ?? "");
            }
        }

        /// <summary>
        /// Can be used to replace CreateRun + SetText
        /// </summary>
        /// <param name="text"></param>
        /// <param name="paragraph"></param>
        public XWPFRun? enrichedText(string? text, XWPFParagraph paragraph)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string[] parts = text.Split("\r\n");
                if (parts?.Length > 0)
                {
                    XWPFRun run = paragraph.CreateRun();
                    bool first = true;
                    foreach (string part in parts)
                    {
                        if (first)
                            first = false;
                        else
                            run.AddCarriageReturn();
                        run.AppendText(part);
                    }
                    return run;
                }
            }
            return null;
        }


        public void resetChapterNumber(int? number = null)
        {
            if (number.HasValue)
                titleNumber = number.Value;
            else
                titleNumber = 1;
        }

        public XWPFParagraph? numberedTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                emptyLine();
                XWPFParagraph paragraph = IncludeParagraph();
                numberedTitleRun(title, paragraph);
                return paragraph;
            }
            else
                return null;
        }
        public XWPFParagraph numberedTitledParagraph(string title, string? text)
        {
            emptyLine();
            XWPFParagraph paragraph1 = IncludeParagraph();
            paragraph1.Alignment = ParagraphAlignment.LEFT;
            numberedTitleRun(title, paragraph1);
            XWPFParagraph paragraph2 = IncludeParagraph();
            paragraph2.Alignment = ParagraphAlignment.LEFT;
            enrichedText(text ?? "", paragraph2);
            return paragraph2;
        }

        public void numberedTitleRun(string title, XWPFParagraph paragraph)
        {
            if (title != null)
            {
                XWPFRun run = paragraph.CreateRun();
                run.IsBold = true;
                run.FontSize = titleFontSize;
                run.AppendText($"{titleNumber++}. ");
                run.AppendText(title);
            }
        }



        //======================
        //       Paragraph
        //----------------------
        public void emptyLine()
        {
            paragraph("");
        }

        public XWPFParagraph paragraph(string? text)
        {
            XWPFParagraph paragraph = IncludeParagraph();
            enrichedText(text, paragraph);
            return paragraph;
        }

        public void paragraphRun(string? text, XWPFParagraph paragraph)
        {
            XWPFRun run = paragraph.CreateRun();
            paragraph.Alignment = ParagraphAlignment.BOTH;
            run.SetText(text ?? "");
        }

        public void pageBreak()
        {
            var paragraph = IncludeParagraph();
            var run = paragraph.CreateRun();
            run.AddBreak(BreakType.PAGE);
        }


        #endregion
        #region Tables Utilities

        //===========================================
        //        Tables Utilities
        //-------------------------------------------

        public XWPFTable createTable(ulong[] colWidths)
        {
            // Quitar para PROCESO
            XWPFTable table = doc.CreateTable(1, colWidths.Length);

            // PROCESO XWPFTable table = doc.CreateTable();
            // PROCESO doc.InsertTable(initialPos++,table);

            ////            CellRangeAddress revisar....
            //CellRangeAddress region = new CellRangeAddress(1, 1, 5, 5);
            //RegionUtil.SetBorderTop(BorderStyle.DashDot, region, Sheet);
            //RegionUtil.SetBorderBottom(BorderStyle.Double, region, sheet);
            //RegionUtil.SetBorderLeft(BorderStyle.Dotted, region, sheet);
            //RegionUtil.SetBorderRight(BorderStyle.SlantedDashDot, region, sheet);

            //RegionUtil.SetTopBorderColor(IndexedColors.Red.Index, region, sheet);
            //RegionUtil.SetBottomBorderColor(IndexedColors.Green.Index, region, sheet);
            //RegionUtil.SetLeftBorderColor(IndexedColors.Blue.Index, region, sheet);
            //RegionUtil.SetRightBorderColor(IndexedColors.Violet.Index, region, sheet);



            //table.SetTopBorder(XWPFTable.XWPFBorderType.SINGLE, 8, 8, "auto");
            //table.SetRightBorder(XWPFTable.XWPFBorderType.SINGLE, 8, 8, "auto");
            //table.SetBottomBorder(XWPFTable.XWPFBorderType.SINGLE, 8, 8, "auto");
            //table.SetLeftBorder(XWPFTable.XWPFBorderType.SINGLE, 8, 8, "auto");

            //table.SetInsideVBorder(XWPFBorderType.SINGLE, 8, 8, "#000000");
            //table.SetInsideHBorder(XWPFBorderType.SINGLE, 8, 8, "#000000");

            setColWidths(table, colWidths);
            setBorders(table);

            return table;
        }

        public void setColWidths(XWPFTable table, ulong[] colWidths)
        {
            var tblLayout1 = table.GetCTTbl().tblPr.AddNewTblLayout();
            tblLayout1.type = ST_TblLayoutType.@fixed;
            for (int i = 0; i < colWidths.Length; i++)
            {
                table.SetColumnWidth(i, colWidths[i]);
            }
        }

        void setBorders(XWPFTable table)
        {
            table.SetCellMargins(100, 150, 100, 150);
            foreach (XWPFTableRow row in table.Rows)
            {
                foreach (XWPFTableCell cell in row.GetTableICells())
                {
                    cell.SetBorderBottom(XWPFBorderType.SINGLE, 5, 0, "000000");
                    cell.SetBorderLeft(XWPFBorderType.SINGLE, 5, 0, "000000");
                    cell.SetBorderRight(XWPFBorderType.SINGLE, 5, 0, "000000");
                    cell.SetBorderTop(XWPFBorderType.SINGLE, 5, 0, "000000");
                }
            }
        }


        public void setCell(XWPFTableRow row, int col, double? number, bool bold = false)
        {
            if (number.HasValue && number.Value > 0)
            {
                //XWPFParagraph paragCell = row.GetCell(col).AddParagraph();
                //paragCell.Alignment = ParagraphAlignment.RIGHT;
                //XWPFRun run = paragCell.CreateRun();
                //run.AppendText(number.HasValue ? number.Value.ToString("#,##0") : "");
                List<XWPFParagraph>? parags = row.GetCell(col)?.Paragraphs.ToList();
                if (parags?.Count > 0)
                {
                    parags[0].Alignment = ParagraphAlignment.RIGHT;
                    parags[0].SpacingAfter = 10;
                    XWPFRun cellRun = parags[0].CreateRun();
                    if (bold)
                        cellRun.IsBold = true;
                    cellRun.SetText(number.HasValue ? number.Value.ToString("#,##0") : "");
                }
            }
        }


        public void cellTextBold(XWPFTableCell cell, string text, ParagraphAlignment alignment)
        {
            List<XWPFParagraph> parags = cell.Paragraphs.ToList();
            if (parags.Count > 0)
            {
                parags[0].Alignment = alignment;
                XWPFRun cellRun = parags[0].CreateRun();
                cellRun.IsBold = true;
                cellRun.SetText(text);
            }
        }

        public void cellTextBold(XWPFTableCell cell, string text1, string text2, string text3, ParagraphAlignment alignment)
        {
            List<XWPFParagraph> parags = cell.Paragraphs.ToList();
            if (parags.Count > 0)
            {
                cellRun(text1, parags[0], alignment, true);
                if (!string.IsNullOrEmpty(text2))
                {
                    XWPFRun? run = cellRun(text2, cell.AddParagraph(), alignment, true);
                }
                if (!string.IsNullOrEmpty(text3))
                {
                    XWPFRun? run = cellRun(text3, cell.AddParagraph(), alignment, false);
                    if (run != null)
                        run.FontSize = 8;
                }
            }
        }

        public void cellTextBold(XWPFTableCell cell, string[] texts, ParagraphAlignment alignment)
        {
            List<XWPFParagraph> parags = cell.Paragraphs.ToList();
            if (parags.Count > 0)
            {
                cellRun(texts[0], parags[0], alignment, true);
                if (texts?.Length > 1)
                {
                    for (int i = 1; i < texts.Length; i++)
                        cellRun(texts[i], cell.AddParagraph(), alignment, true);
                }
            }
        }

        public void cellText(XWPFTableCell cell, string? text, ParagraphAlignment alignment)
        {
            List<XWPFParagraph> parags = cell.Paragraphs.ToList();
            if (parags.Count > 0)
            {
                parags[0].Alignment = alignment;
                enrichedText(text, parags[0]);
            }
        }

        public void cellText(XWPFTableCell cell, string text1, string text2, string text3, ParagraphAlignment alignment)
        {
            List<XWPFParagraph> parags = cell.Paragraphs.ToList();
            if (parags.Count > 0)
            {
                if (!string.IsNullOrEmpty(text1))
                    cellRun(text1, parags[0], alignment, false);
                if (!string.IsNullOrEmpty(text2))
                {
                    XWPFRun? run = cellRun(text2, cell.AddParagraph(), alignment, false);
                }
                if (!string.IsNullOrEmpty(text3))
                {
                    XWPFRun? run = cellRun(text3, cell.AddParagraph(), alignment, false);
                    if (run != null)
                        run.FontSize = 8;
                }
            }
        }

        public XWPFRun? cellRun(string text, XWPFParagraph paragraph, ParagraphAlignment alignment, bool bold)
        {
            if (!string.IsNullOrEmpty(text))
            {
                paragraph.Alignment = alignment;
                paragraph.SpacingAfterLines = 4;
                XWPFRun? run = enrichedText(text, paragraph);
                if (bold && run != null)
                    run.IsBold = true;
                return run;
            }
            return null;
        }

        #endregion
        #region Other Utilities

        //================================================================
        //
        //           O t h e r   L i b s
        //
        //----------------------------------------------------------------
        public double? add(double? number1, double? number2)
        {
            if (number1 == null)
                return number2;
            else
                return number1 + (number2 ?? 0);
        }

        public string display(DateTime? date)
        {
            if (date.HasValue)
                if (date != DateTime.MinValue)
                    return date.Value.ToString("yyyy-MM-dd");
            return "";
        }

        public string displayMonth(DateTime? date)
        {
            if (date.HasValue)
                if (date != DateTime.MinValue)
                    return date.Value.ToString("MM-yyyy");
            return "";
        }


        #endregion
    }
}



