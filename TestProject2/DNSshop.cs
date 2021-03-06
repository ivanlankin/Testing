using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutomatedTests
{
    public class DNSshopTests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://www.dns-shop.ru/");
        }

        [Test]
        public void TestPriceFilter()
        {
            driver.FindElement(By.XPath("//*[@class='menu-desktop__root']//a[contains(text(), 'ноутбуки')]")).Click();
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//input[contains(@placeholder, 'от 12999')]")).Clear();
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//input[contains(@placeholder, 'от 12999')]")).SendKeys("20000");
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//input[contains(@placeholder, 'до 334999')]")).Clear();
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//input[contains(@placeholder, 'до 334999')]")).SendKeys("40000");
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//input[contains(@placeholder, 'до 334999')]")).SendKeys(Keys.Enter);
            new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                .Until(x => driver.FindElements(By.CssSelector(".catalog-preloader__spin")).Count == 0);
        
            int[] actualValues = Array.ConvertAll(driver.FindElements(By.XPath("//*[contains(@class,'catalog-products')]//*[@class='product-buy__price']"))
               .Select(webPrice => webPrice.Text.Replace(" ", "").Trim('₽')).ToArray<string>(), s => int.Parse(s));
            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= 20000 && actualPrice <= 40000, "Price filter works wrong. Actual price is " + actualPrice + ". But should be more or equal than 20000 and less or equal than 40000"));
        }
        
        [Test]
        public void TestTooltipText()
        {
            driver.FindElement(By.XPath("//*[@class='menu-desktop__root']//a[contains(text(), 'ноутбуки')]")).Click();
            new Actions(driver).MoveToElement(driver.FindElement(By.XPath("//*[contains(@class,'catalog-product__buy')]//*[contains(@class, 'wishlist-btn')]"))).Build().Perform();
            Assert.IsTrue(driver.FindElements(By.XPath("//*[contains(@class,'catalog-product__buy')]//*[@class='button-ui buy-btn button-ui_brand']")).Any(),
                "Tooltip has not appeared.");
            Assert.AreEqual("Добавить в избранное", driver.FindElement(By.XPath("//*[@class='tooltip-ui tooltip-ui_top']")).Text.Trim(),
                "Tooltip has not appeared.");
        }

        [Test]
        public void NegativeSignUpTest()
        {
            driver.FindElement(By.CssSelector(".header__login_button")).Click();
            driver.FindElement(By.CssSelector(".base-ui-input-row__input")).Click();
            driver.FindElement(By.CssSelector(".base-ui-input-row__input")).SendKeys("vfbdhjsk57bs442");
            driver.FindElement(By.XPath("//*[@class='form-entry-or-registry__main-button']//*[contains(@class,'base-ui-button')]")).Click();
            Assert.AreEqual("E-mail/телефон указан неверно", driver.FindElement(By.CssSelector(".error-message-block.form-entry-or-registry__error")).Text.Trim(),
                    "Registration is possible when phone number input has incorrect value.");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}