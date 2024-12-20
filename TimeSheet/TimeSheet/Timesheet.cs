using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Timesheet
{
    class Timesheet
    {
        static void Main(string[] args)
        {
            //ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            //string excelPath = @"C:\Users\RaphaelGundim\Desktop\Timesheet.xlsx";

            string excelPath = ConfigurationManager.AppSettings["ExcelPath"];
            string url = ConfigurationManager.AppSettings["Url"];
            string login = ConfigurationManager.AppSettings["Login"];
            string password = ConfigurationManager.AppSettings["Password"];
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;



            var excelBot = new ExcelBot(excelPath);
            var records = excelBot.ReadExcelFile();

            var timesheetBot = new TimesheetBot("https://itworks.com.br/controle", "raphael.gundim@gmail.com", "raphael@gundim1234");

            timesheetBot.Login();
            timesheetBot.ProcessarInformacoes(records);
            timesheetBot.Quit();
        }
    }
    public class Record
    {
        public string Data { get; set; }
        public string Chamado { get; set; }
        public string Servico { get; set; }
        public string Categoria { get; set; }
        public string Subcategoria { get; set; }
        public string Inicio { get; set; }
        public string Fim { get; set; }
        public string Descricao { get; set; }
    }
}
