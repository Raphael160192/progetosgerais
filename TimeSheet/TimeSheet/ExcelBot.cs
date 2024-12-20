using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using static Timesheet.Timesheet;

namespace Timesheet
{
    public class ExcelBot
    {
        private string excelPath;

        public ExcelBot(string excelPath)
        {
            this.excelPath = excelPath;
        }

        public List<Record> ReadExcelFile()
        {
            var records = new List<Record>();
            FileInfo fileInfo = new FileInfo(excelPath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string inicio = worksheet.Cells[row, 6].Text;
                    string fim = worksheet.Cells[row, 7].Text;
                    inicio = FormatTime(inicio);
                    fim = FormatTime(fim);

                    records.Add(new Record
                    {
                        Data = worksheet.Cells[row, 1].Text,
                        Chamado = worksheet.Cells[row, 2].Text,
                        Servico = worksheet.Cells[row, 3].Text,
                        Categoria = worksheet.Cells[row, 4].Text,
                        Subcategoria = worksheet.Cells[row, 5].Text,
                        Inicio = inicio,
                        Fim = fim,
                        Descricao = worksheet.Cells[row, 8].Text,
                    });
                }
            }
            return records;
        }

        private string FormatTime(string time)
        {
            if (string.IsNullOrEmpty(time))
            {
                return "";
            }

            if (TimeSpan.TryParse(time, out TimeSpan timeSpan))
            {
                return timeSpan.ToString(@"hh\:mm\:ss");
            }
            else
            {
                return time;
            }
        }
    }
}
