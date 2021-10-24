using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace BloomerangPaymentForm
{
    [TestFixture]
    public class BloomerangDonationFormTests
    {
        const int WaitTimeoutSeconds = 10;
        const string SubmitButtonSelector = "express-submit";
        const string SavingsPaymentInformationSelector = "Savings";
        const string DonationLevelSelector = "donation-level";
        const string BankFeesSelector = "true-impact";

        private ChromeDriver driver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Open the browser and navigate to donation form
            driver = new ChromeDriver();
            GoToDonationForm();
        }

        [SetUp]
        public void SetUp()
        {
            // Open the browser and navigate to donation form
            driver.Navigate().Refresh();
            WaitUntilElementIsDisplayed(By.Id(SubmitButtonSelector));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Close the browser
            driver.Dispose();
        }

        [Test(Description = "Test Case 1.1: Verify Donate button displays $1.00 radio button donation selection with Savings payment information")]
        public void Given1DollarPaymentOption_WhenSavingsPaymentInformation_ThenFinalDonationIsOneDollar()
        {
            driver.FindElementsByName(DonationLevelSelector).First(e => e.GetAttribute("value") == "1.000000").Click();
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $1.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.2: Verify Donate button displays $2.00 radio button donation selection with Savings payment information")]
        public void Given2DollarPaymentOption_WhenSavingsPaymentInformation_ThenFinalDonationIsTwoDollars()
        {
            driver.FindElementsByName(DonationLevelSelector).First(e => e.GetAttribute("value") == "2.000000").Click();
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $2.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.3: Verify Donate button displays $100.00 radio button donation selection with Savings payment information")]
        public void Given100DollarPaymentOption_WhenSavingsPaymentInformation_ThenFinalDonationIsOneHundredDollars()
        {
            driver.FindElementsByName(DonationLevelSelector).First(e => e.GetAttribute("value") == "100.000000").Click();
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $100.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.4: Verify Donate button displays Other amount donation selection when amount is greater than $1.00 with Savings payment information")]
        public void GivenOtherDollarPaymentOption_WhenSavingsPaymentInformation_ThenFinalDonationDisplaysOtherAmountDollars()
        {
            driver.FindElementById("other-option").Click();
            driver.FindElementById("other-amount").SendKeys("20");
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $20.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.5: Verify Donate button displays Other amount donation selection when amount is equal $1.00 with Savings payment information")]
        public void GivenOtherPaymentOptionHas1DollarValue_WhenSavingsPaymentInformation_ThenFinalDonationIsOneDollar()
        {
            driver.FindElementById("other-option").Click();
            driver.FindElementById("other-amount").SendKeys("1");
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $1.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.6: Verify Donate button displays 'Donate $0.00' when user selects Other donation selection when value is 0 with Savings payment information")]
        public void GivenOtherPaymentOptionHas0DollarValue_WhenSavingsPaymentInformation_ThenISeeInvalidCharactersErrorMessageAndFinalDonationIsZeroDollars()
        {
            driver.FindElementById("other-option").Click();
            driver.FindElementById("other-amount").SendKeys("0");
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            var invalidAmountElement = driver.FindElementByXPath("//label[text()='Invalid amount']");
            Assert.NotNull(invalidAmountElement, "'Invalid amount' error message is not displayed");
            Assert.AreEqual("Donate $0.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.7: Verify Donate button displays 'Donate $0.00' when user selects Other donation selection when amount is greater than 0 and less then 1.00 with Savings payment information")]
        public void GivenOtherPaymentOptionIsLessThan1AndGreaterThan0_WhenSavingsPaymentInformation_ThenFinalDonationIsZeroDollars()
        {
            driver.FindElementById("other-option").Click();
            driver.FindElementById("other-amount").SendKeys("0.5");
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $0.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.8: Verify Donate button does not display invalid characters with Savings payment information")]
        public void GivenInvalidCharactersAreEnteredInOtherPaymentOption_WhenSavingsPaymentInformation_ThenISeeInvalidCharactersErrorMessageAndFinalDonationIsZeroDollars()
        {
            driver.FindElementById("other-option").Click();
            driver.FindElementById("other-amount").SendKeys("adsdsfs");
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            var invalidAmountElement = driver.FindElementByXPath("//label[text()='Invalid amount']");
            Assert.NotNull(invalidAmountElement, "'Invalid amount' error message is not displayed");
            Assert.AreEqual("Donate $0.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.9: Verify Donate button displays amount chosen by Donation option even Other textbox contains different value with Savings payment information")]
        public void Given1DollarPaymentOptionSelectedAndOtherPaymentOptionContains50AtTheSameTime_WhenSavingsPaymentInformation_ThenFinalDonationDisplays1Dollar()
        {
            driver.FindElementsByName(DonationLevelSelector).First(e => e.GetAttribute("value") == "1.000000").Click();
            driver.FindElementById("other-amount").SendKeys("50");
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $1.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.10: Verify Donate button does not display negative amount with Savings payment information")]
        public void GivenOtherPaymentOptionContainsNegativeValue_WhenSavingsPaymentInformation_ThenFinalDonationIsZeroDollars()
        {
            driver.FindElementById("other-option").Click();
            driver.FindElementById("other-amount").SendKeys("-10");
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $0.00", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.11: Verify Donate button calculates $1.00 radio button donation selection and bank fees with Savings payment information")]
        public void Given1DollarPaymentOptionAndIncreaseMyImpactAreSelected_WhenSavingsPaymentInformation_ThenFinalDonationIsOneDollarAndTwentyCents()
        {
            driver.FindElementsByName(DonationLevelSelector).First(e => e.GetAttribute("value") == "1.000000").Click();
            driver.FindElementById(BankFeesSelector).Click();
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $1.20", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.12: Verify Donate button calculates $2.00 radio button donation selection and bank fees with Savings payment information")]
        public void Given2DollarsPaymentOptionAndIncreaseMyImpactAreSelected_WhenSavingsPaymentInformation_ThenFinalDonationIsTwoDollarsAndTwentyCents()
        {
            driver.FindElementsByName(DonationLevelSelector).First(e => e.GetAttribute("value") == "2.000000").Click();
            driver.FindElementById(BankFeesSelector).Click();
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $2.20", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.13: Verify Donate button calculates $100.00 radio button donation selection and bank fees with Savings payment information")]
        public void Given100DollarsPaymentOptionAndIncreaseMyImpactAreSelected_WhenSavingsPaymentInformation_ThenFinalDonationIsOneHundredDollarsAndTwentyCents()
        {
            driver.FindElementsByName(DonationLevelSelector).First(e => e.GetAttribute("value") == "100.000000").Click();
            driver.FindElementById(BankFeesSelector).Click();
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $100.20", GetDonationButtonText());
        }

        [Test(Description = "Test Case 1.14: Verify Donate button calculates other-amount donation selection and bank fees when amount is greater than $1.00 with Savings payment information")]
        public void GivenOtherPaymentOptionIsSelectedAndContains20AndIncreaseMyImpactIsSelected_WhenSavingsPaymentInformation_ThenFinalDonationIsTwentyDollarsAndTwentyCents()
        {
            driver.FindElementById("other-option").Click();
            driver.FindElementById("other-amount").SendKeys("20");
            driver.FindElementById(BankFeesSelector).Click();
            driver.FindElementById(SavingsPaymentInformationSelector).Click();

            Assert.AreEqual("Donate $20.20", GetDonationButtonText());
        }

        private void GoToDonationForm()
        {
            var url = "https://crm.bloomerang.co/HostedDonation?ApiKey=pub_797712d5-2155-11e8-94a1-0a7fa948a058&WidgetId=434176";
            driver.Navigate().GoToUrl(url);
        }

        public IWebElement WaitUntilElementIsDisplayed(By by)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WaitTimeoutSeconds));
            return wait.Until(d => d.FindElement(by));
        }

        private string GetDonationButtonText()
        {
            return driver.FindElementById(SubmitButtonSelector).GetAttribute("value");
        }

        private void WaitSeconds(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }
    }
}
