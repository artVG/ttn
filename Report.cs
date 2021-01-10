using System;
using System.Collections.Generic;
using ClosedXML.Excel;

namespace ttn
{
    /// <summary>
    /// Excel workbook with report
    /// </summary>
    class Report
    {
        XLWorkbook workbook;

        /// <summary>
        /// creates new workbook
        /// </summary>
        public Report()
        {
            workbook = new XLWorkbook();
        }

        /// <summary>
        /// create a list in workbook with 'name' and write 'document' ranges to it
        /// </summary>
        /// <param name="documents">list of documents to write</param>
        /// <param name="name">list name</param>
        public void WriteList(List<Document[]> documents, string name)
        {
            WriteRange(documents, name);
        }

        /// <summary>
        /// write report workbook to specified location
        /// </summary>
        /// <param name="path">path to write workbook</param>
        public void WriteReport(string path)
        {
            //replace forbidden characters from file name 
            string name = String.Format("-отчет-{0}", DateTime.Now
                                                        .ToString()
                                                        .Replace(':', '-')
                                                        .Replace('/', '-'));
            //save file with xlsx extension
            workbook.SaveAs(path + name + ".xlsx");
        }

        /// <summary>
        /// write given range to this->workbook Will create new worksheet
        /// </summary>
        /// <param name="documents">list of documents to write</param>
        /// <param name="name">Name of the worksheet. Header for document range</param>
        private void WriteRange(List<Document[]> documents, string name)
        {
            //create new worksheet in the workbook
            IXLWorksheet worksheet = workbook.Worksheets.Add(name);
            //add range header ROW_1
            worksheet.Cell("A1").Value = name;
            //create table headline ROW_2
            worksheet.Cell("A2").Value = "Количество";
            worksheet.Cell("B2").Value = "Серия";
            worksheet.Range(worksheet.Cell("C2"), worksheet.Cell("D2")).Merge()
                .Value = "Номера";
            //starting at ROW_3 write ranges
            int currentRow = 3;
            foreach (Document[] docRange in documents)
            {
                //number of documents in range
                worksheet.Cell(String.Format("A{0}", currentRow))
                    .Value = docRange[0].CountAmountTo(docRange[1]);
                //Series of documents
                worksheet.Cell(String.Format("B{0}", currentRow))
                    .Value = docRange[0].Series;
                //if documents range includes only one document
                if (docRange[0] == docRange[1])
                {
                    //merge first and last cells and write document number
                    worksheet.Range(worksheet.Cell(String.Format("C{0}", currentRow)), 
                                    worksheet.Cell(String.Format("D{0}", currentRow))).Merge()
                        .Value = docRange[0].Number;
                }
                //if documents range includes more than one document
                else
                {
                    //write first document number to the first cell
                    worksheet.Cell(String.Format("C{0}", currentRow))
                        .Value = docRange[0].Number;
                    //write first document number to the last cell
                    worksheet.Cell(String.Format("D{0}", currentRow))
                        .Value = docRange[1].Number;
                }
                //go to the next row
                currentRow++;
            }
        }
    }
}
