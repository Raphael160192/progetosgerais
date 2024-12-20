using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;

class ReembolsoAutomacao
{
    static void Main(string[] args)
    {
        string caminhoPlanilha = @"C:\Users\RaphaelGundim\Desktop\Reembolso\TESTE ROBO.xlsx";
        var registros = CarregarPlanilha(caminhoPlanilha);
        IWebDriver driver = new ChromeDriver();

        if (LoginSistema(driver, "https://alpheratz.itapevarec.com.br/Alpheratz/login.aspx", "jndsantos", "Eys@2025@finan"))
        {
            foreach (var registro in registros)
            {
                try
                {
                    ProcessarRegistro(driver, registro);
                    registro.Status = "Concluído"; // Atualiza status para Concluído
                }
                catch (Exception ex)
                {
                    registro.Status = "Processado com falhas"; // Atualiza status para falhas
                    registro.MotivoErro = ex.Message; // Registra o motivo do erro
                }
            }

            // Atualiza a planilha com os resultados do processamento
            AtualizarPlanilha(caminhoPlanilha, registros);
            Console.WriteLine("Processamento finalizado.");
            driver.Quit();
        }
        else
        {
            Console.WriteLine("Falha no login. Processo abortado.");
        }
    }

    public class RegistroReembolso
    {
        public string Correspondente { get; set; }
        public string Pasta { get; set; }
        public string NumeroProcesso { get; set; }
        public string Parte { get; set; }
        public string DescricaoSistema { get; set; }
        public string DescricaoReembolso { get; set; }
        public DateTime DataRecibo { get; set; }
        public string ValorReembolso { get; set; }
        public string Status { get; set; }
        public string MotivoErro { get; set; }
        public string CaminhoArquivo { get; set; }
    }

    public static List<RegistroReembolso> CarregarPlanilha(string caminhoPlanilha)
    {
        var registros = new List<RegistroReembolso>();

        using (var package = new ExcelPackage(new FileInfo(caminhoPlanilha)))
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                // Verifica se a coluna C (terceira coluna) está vazia
                if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 3].Text))
                {
                    continue; // Pula a linha se a coluna C estiver vazia
                }

                var registro = new RegistroReembolso
                {
                    Correspondente = worksheet.Cells[row, 1].Text,
                    Pasta = worksheet.Cells[row, 2].Text,
                    NumeroProcesso = worksheet.Cells[row, 3].Text,
                    Parte = worksheet.Cells[row, 4].Text,
                    DescricaoSistema = worksheet.Cells[row, 5].Text.Trim(),
                    DescricaoReembolso = worksheet.Cells[row, 6].Text,
                    Status = worksheet.Cells[row, 11].Text
                };

                // Tenta converter o valor da célula de DataRecibo para DateTime
                string dataTexto = worksheet.Cells[row, 7].Text;
                if (DateTime.TryParse(dataTexto, out DateTime dataRecibo))
                {
                    registro.DataRecibo = dataRecibo;
                }
                else
                {
                    registro.Status = "Processado com falhas";
                    registro.MotivoErro = $"Data inválida na linha {row}: '{dataTexto}'";
                }

                string valorTexto = worksheet.Cells[row, 8].Text.Replace("R$", "").Trim();
                registro.ValorReembolso = valorTexto.Replace(".", "").Replace(',', '.');

                // Adiciona apenas os registros não processados ou com falhas
                if (string.IsNullOrEmpty(registro.Status) || registro.Status == "Processado com falhas")
                {
                    registros.Add(registro);
                }
            }
        }

        return registros;
    }



    public static bool LoginSistema(IWebDriver driver, string url, string usuario, string senha)
    {
        try
        {
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
            driver.FindElement(By.Id("txt_username")).SendKeys(usuario);
            driver.FindElement(By.Id("txt_password")).SendKeys(senha);
            driver.FindElement(By.Id("btn_login")).Click();

            return driver.Url.Contains("default.aspx");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao tentar login: {ex.Message}");
            return false;
        }
    }

    public static void ProcessarRegistro(IWebDriver driver, RegistroReembolso registro)
    {
        try
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            IWebElement dropdown = driver.FindElement(By.Id("ctl00_ddl_mainmenu"));
            SelectElement selectLegal = new SelectElement(dropdown);
            selectLegal.SelectByText("> Legal");

            wait.Until(ExpectedConditions.UrlContains("legal.aspx"));

            IWebElement numeroProcessoInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_content_txt_search")));
            numeroProcessoInput.SendKeys(registro.NumeroProcesso);

            IWebElement buscarButton = driver.FindElement(By.Id("ctl00_content_bt_litigation_search"));
            buscarButton.Click();

            IWebElement primeiroLitigationID = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("table.box-table-a tbody tr a")));
            primeiroLitigationID.Click();

            IWebElement despesasButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ctl00_content_bt_expenses")));
            despesasButton.Click();

            IWebElement acrescentarDespesaButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("bt_addExpense")));
            acrescentarDespesaButton.Click();

            IWebElement tipoDespesaDropdown = driver.FindElement(By.Id("ctl00_content_ddl_ExpenseTypeName"));
            SelectElement selectElement = new SelectElement(tipoDespesaDropdown);
            selectElement.SelectByText(GetMatchingOption(selectElement, registro.DescricaoSistema));

            IWebElement dataDespesaInput = driver.FindElement(By.Id("ctl00_content_txt_ExpenseDate"));
            dataDespesaInput.Clear();
            dataDespesaInput.SendKeys(registro.DataRecibo.ToString("dd/MM/yyyy"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("document.getElementById('ui-datepicker-div').style.display = 'none';");

            IWebElement valorDespesaInput = driver.FindElement(By.Id("ctl00_content_txt_ExpenseAmount"));
            //valorDespesaInput.SendKeys(registro.ValorReembolso.ToString("C2"));
            valorDespesaInput.SendKeys(registro.ValorReembolso);

            IWebElement descricaoInput = driver.FindElement(By.Id("ctl00_content_txt_ExpenseDescription"));
            descricaoInput.SendKeys(registro.DescricaoReembolso);

            IWebElement fileInput = driver.FindElement(By.CssSelector("input[type='file']"));
            string nomeArquivoOriginal = $"{registro.Pasta} - {registro.ValorReembolso.Replace("R$", "").Trim()}.pdf";
            string nomeArquivo = nomeArquivoOriginal.Replace(",", ".");
            string caminhoArquivo = $@"C:\Users\RaphaelGundim\Desktop\Reembolso\{nomeArquivo}";

            if (File.Exists(caminhoArquivo))
            {
                fileInput.SendKeys(caminhoArquivo);
                bool uploadConcluido = wait.Until(driver =>
                {
                    try
                    {
                        // Relocaliza o elemento da lista de uploads
                        IWebElement uploadStatus = driver.FindElement(By.CssSelector("ul.qq-upload-list > li"));
                        string statusClass = uploadStatus.GetAttribute("class");
                        return statusClass.Contains("qq-upload-success") || statusClass.Contains("qq-upload-failed");
                    }
                    catch (NoSuchElementException)
                    {
                        // Upload ainda não iniciado
                        return false;
                    }
                });

                if (!uploadConcluido)
                {
                    registro.Status = "Processado com falhas";
                    registro.MotivoErro = "Upload não foi concluído.";
                    driver.Navigate().GoToUrl("https://alpheratz.itapevarec.com.br/Alpheratz/intranet/default.aspx");
                    return;
                }

                // Verifica se o upload falhou
                IWebElement statusUpload = driver.FindElement(By.CssSelector("ul.qq-upload-list > li"));
                string statusTexto = statusUpload.Text;
                if (statusTexto.Contains("Erro ao executar o upload"))
                {
                    registro.Status = "Processado com falhas";
                    registro.MotivoErro = "Erro ao executar o upload do arquivo.";
                    driver.Navigate().GoToUrl("https://alpheratz.itapevarec.com.br/Alpheratz/intranet/default.aspx");
                    return;
                }
            }
            else
            {
                registro.Status = "Processado com falhas";
                registro.MotivoErro = $"Arquivo não encontrado: {caminhoArquivo}";
                driver.Navigate().GoToUrl("https://alpheratz.itapevarec.com.br/Alpheratz/intranet/default.aspx");
                return;
            }

            IWebElement salvarButton = driver.FindElement(By.Id("bt_save_Expense"));
            salvarButton.Click();

            bool urlMudouParaSucesso = wait.Until(driver =>
            {
                string currentUrl = driver.Url;
                return currentUrl.Contains("legal.aspx?action=go&id=");
            });

            if (urlMudouParaSucesso)
            {
                registro.Status = "Concluído";
            }
            else
            {
                registro.Status = "Processado com falhas";
                registro.MotivoErro = "URL de sucesso não foi atingida após o salvamento.";
                driver.Navigate().GoToUrl("https://alpheratz.itapevarec.com.br/Alpheratz/intranet/default.aspx");
            }

            driver.Navigate().GoToUrl("https://alpheratz.itapevarec.com.br/Alpheratz/intranet/default.aspx");
            return;
        }
        catch (NoSuchElementException ex)
        {
            throw new Exception($"Elemento não encontrado no registro {registro.NumeroProcesso}: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro no processamento do registro {registro.NumeroProcesso}: {ex.Message}");
        }
    }

    public static string GetMatchingOption(SelectElement select, string descricao)
    {
        foreach (var option in select.Options)
        {
            if (option.Text.Trim().Equals(descricao.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return option.Text;
            }
        }
        return null;
    }

    public static void AtualizarPlanilha(string caminhoPlanilha, List<RegistroReembolso> registros)
    {
        using (var package = new ExcelPackage(new FileInfo(caminhoPlanilha)))
        {
            var worksheet = package.Workbook.Worksheets[0];

            foreach (var registro in registros)
            {
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    if (worksheet.Cells[row, 3].Text == registro.NumeroProcesso)
                    {
                        worksheet.Cells[row, 12].Value = registro.Status;
                        worksheet.Cells[row, 13].Value = registro.MotivoErro;
                        break;
                    }
                }
            }

            package.Save();
        }
    }
}
