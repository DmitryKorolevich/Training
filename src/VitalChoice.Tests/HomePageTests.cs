using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace VitalChoice.Tests
{
    public class HomePageTests
    {
        private static readonly ChromeDriver ChromeDriver;

        static HomePageTests()
        {
            ChromeDriver = new ChromeDriver();
        }

        [Fact]
        public void HomePageTest()
        {
            ChromeDriver.Navigate().GoToUrl("https://staging.g2-dg.com/");
            var element = ChromeDriver.FindElementById("slider");
            Assert.Equal(element.GetAttribute("class"), "nivoSlider");
        }
    }
}