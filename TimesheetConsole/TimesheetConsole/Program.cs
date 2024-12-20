using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Timesheet
{
    class Timesheet
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string excelPath = @"C:\Users\RaphaelGundim\Desktop\Timesheet.xlsx";
            var records = ReadExcelFile(excelPath);

            var options = new EdgeOptions();
            IWebDriver driver = new EdgeDriver(options);
            //IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://itworks.com.br/controle");
            driver.Manage().Window.Maximize();

            // Login
            driver.FindElement(By.Name("email")).SendKeys("raphael.gundim@gmail.com");
            driver.FindElement(By.Name("senha")).SendKeys("raphael@gundim1234");
            driver.FindElement(By.Id("m_login_signin_submit")).Click();

            // Wait for login to complete, assuming a successful login redirects to /controle
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.Url == "https://itworks.com.br/controle/inicio");

            foreach (var record in records)
            {
                try
                {
                    ProcessRecord(driver, record);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar o registro: {ex.Message}");
                }
            }

            driver.Quit();
        }

        static List<Record> ReadExcelFile(string path)
        {
            var records = new List<Record>();
            FileInfo fileInfo = new FileInfo(path);

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

        static string FormatTime(string time)
        {
            // Verifica se a string de tempo está vazia
            if (string.IsNullOrEmpty(time))
            {
                return "";
            }

            // Formata a hora no formato "00:00:00"
            if (TimeSpan.TryParse(time, out TimeSpan timeSpan))
            {
                return timeSpan.ToString(@"hh\:mm\:ss");
            }
            else
            {
                // Trate qualquer erro de formatação aqui
                return time;
            }
        }

        static void ProcessRecord(IWebDriver driver, Record record)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));

            // Espera até que o modal desapareça antes de tentar clicar no link "Lançamentos"
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".modal-dialog")));

            // Acessar a página de lançamentos
            wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Lançamentos"))).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[data-target='#adicionar']"))).Click();

            // Preencher o formulário no modal
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("data_inicio_cad"))).Clear();
            driver.FindElement(By.Id("data_inicio_cad")).SendKeys(record.Data);
            driver.FindElement(By.Id("hora_inicio_cad")).Clear();
            driver.FindElement(By.Id("hora_inicio_cad")).SendKeys(record.Inicio);
            driver.FindElement(By.Id("hora_fim_cad")).SendKeys(record.Fim);

            // Preencher Centro de Custo
            var centroCustoDropDown = new SelectElement(driver.FindElement(By.Id("centro_cad")));
            centroCustoDropDown.SelectByText("IT Works");

            // Preencher Serviço
            var servicoDropDown = new SelectElement(driver.FindElement(By.Id("servico_cad")));
            wait.Until(d => servicoDropDown.Options.Count > 1); // Esperar até que haja opções disponíveis
            //System.Threading.Thread.Sleep(3000); // Esperar 3 segundos para garantir o carregamento do dropdown
            try
            {
                if (record.Servico.Equals("Turbo SPED", StringComparison.OrdinalIgnoreCase))
                {
                    servicoDropDown.SelectByIndex(1);
                }
                else if (record.Servico.Equals("e-Malote", StringComparison.OrdinalIgnoreCase))
                {
                    servicoDropDown.SelectByIndex(2);
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Erro ao processar o registro: Serviço '{record.Servico}' não encontrado.");
                return;
            }

            // Preencher Categoria e Subcategoria
            var categoriaDropDown = new SelectElement(driver.FindElement(By.Id("categoria_cad")));
            wait.Until(d => categoriaDropDown.Options.Count > 1); // Esperar até que haja opções disponíveis

            try
            {
                if (record.Categoria.Equals("Suporte", StringComparison.OrdinalIgnoreCase))
                {
                    categoriaDropDown.SelectByIndex(1);
                }
                else
                {
                    categoriaDropDown.SelectByIndex(2);
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Erro ao processar o registro: Categoria ou Subcategoria não encontrada para '{record.Categoria}'");
                return;
            }

            var subcategoriaDropDown = new SelectElement(driver.FindElement(By.Id("subcategoria_cad")));
            wait.Until(d => subcategoriaDropDown.Options.Count > 1); // Esperar até que haja opções disponíveis

            try
            {
                if (record.Subcategoria.Equals("Análise Suporte", StringComparison.OrdinalIgnoreCase))
                {
                    subcategoriaDropDown.SelectByIndex(1);
                }
                else if (record.Subcategoria.Equals("Desenvolvimento", StringComparison.OrdinalIgnoreCase))
                {
                    subcategoriaDropDown.SelectByIndex(2);
                }
                else
                {
                    subcategoriaDropDown.SelectByIndex(17);
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Erro ao processar o registro: Categoria ou Subcategoria não encontrada para subcategoria '{record.Subcategoria}'.");
                return;
            }

            // Preencher Título e Descrição
            driver.FindElement(By.Id("assunto_cad")).SendKeys($"Chamado - {record.Chamado}");
            driver.FindElement(By.Id("descricao_cad")).SendKeys(record.Descricao);

            // Submeter o formulário
            driver.FindElement(By.XPath("//button[text()='Adicionar tarefa']")).Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".modal-dialog")));

            // Adicionar uma pequena espera após fechar o modal para garantir que todas as operações estejam completas
            System.Threading.Thread.Sleep(1000);
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
