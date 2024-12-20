using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using static Timesheet.Timesheet;

namespace Timesheet
{
    public abstract class BotBase
    {
        protected IWebDriver driver;
        protected string url;
        protected string login;
        protected string senha;

        public BotBase(string url, string login, string senha)
        {
            this.url = url;
            this.login = login;
            this.senha = senha;
            ConfigurarDriver();
        }

        private void ConfigurarDriver()
        {
            var options = new EdgeOptions();
            driver = new EdgeDriver(options);
        }

        public void Login()
        {
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
            driver.FindElement(By.Name("email")).SendKeys(login);
            driver.FindElement(By.Name("senha")).SendKeys(senha);
            driver.FindElement(By.Id("m_login_signin_submit")).Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.Url == url + "/inicio");
        }

        public abstract void ProcessarInformacoes(List<Record> records);

        public void Quit()
        {
            driver.Quit();
        }
    }
}
