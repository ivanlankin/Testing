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
    public class Tests
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
            driver.FindElement(By.XPath("//*[@class='menu-desktop__root'][4]//a[2]")).Click();
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//div[1]/input")).Clear();
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//div[1]/input")).SendKeys("20000");
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//div[2]/input")).Clear();
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//div[2]/input")).SendKeys("40000");
            driver.FindElement(By.XPath("//*[contains(@class, 'ui-collapse__content')]//div[2]/input")).SendKeys(Keys.Enter);
            new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                .Until(x => driver.FindElements(By.CssSelector(".catalog-preloader__spin")).Count == 0);

            int[] actualValues = Array.ConvertAll(driver.FindElements(By.XPath("//*[contains(@class,'catalog-products')]//*[@class='product-buy__price']"))
               .Select(webPrice => webPrice.Text.Replace(" ", "").Trim('₽')).ToArray<string>(), s => int.Parse(s));
            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= 20000 && actualPrice <= 40000, "Price filter works wrong. Actual price is " + actualPrice + ". But should be more or equal than 20000 and less or equal than 40000"));
        }

        [Test]
        public void TestTooltipText()
        {
            new Actions(driver).MoveToElement(driver.FindElement(By.XPath("[contains(@class, 'wishlist-btn')]"))).Build().Perform();
            Assert.IsTrue(driver.FindElements(By.XPath("//*[@class='products-list']//*[@class='button-ui buy-btn button-ui_brand']")).Any(),
                "Tooltip has not appeared.");
            Assert.AreEqual("Добавить в избранное", driver.FindElement(By.XPath("//*[@class='tooltip-ui tooltip-ui_top']")).Text.Trim(),
                "Tooltip has not appeared.");
        }

        //[Test]
        //public void NegativeSignUpTest()
        //{
        //    driver.FindElement(By.CssSelector(".AuthPopup__button")).Click();
        //    driver.FindElement(By.CssSelector(".AuthGroup__tab-sign-up")).Click();
        //    driver.FindElement(By.CssSelector(".js--SignUp__input-name__container-input")).SendKeys("Test");
        //    driver.FindElement(By.CssSelector(".js--SignUp__input-email__container-input")).SendKeys("vfbdhjsk57bs442@mail.ru");
        //    Assert.IsFalse(driver.FindElements(By.XPath("//button[contains(@class, 'SignUp__button-confirm-phone') and not(@disabled)]")).Any(),
        //        "Phone number confirmation button is enabel when phone number input has no value.");
        //}

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}