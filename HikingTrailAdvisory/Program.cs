using AngleSharp;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V119.Network;
using OpenQA.Selenium.Support.UI;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace HikingTrailAdvisory
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string baseUrl = "https://www.wta.org/go-outside/hikes";
            string pattern = @"[^/]+$"; // What is the pattern for?
            Dictionary<string, Hike> hikesDict = new Dictionary<string, Hike>();

            // Set up Driver for web scraping
            new DriverManager().SetUpDriver(new ChromeConfig());
            IWebDriver driver = new ChromeDriver();

            // Go to base url (WTA website with hiking trails)
            driver.Navigate().GoToUrl(baseUrl);

            try
            {
                while (true)
                {
                    // Click through all pages and populate hikesDict with all hike names / links
                    HikeInfoScraper.ScrapeHikeLinks(driver, pattern, hikesDict);
                }
            }
            catch (StaleElementReferenceException staleEx)
            {
                Console.WriteLine(staleEx.ToString());
                driver = new ChromeDriver();
                HikeInfoScraper.ScrapeHikeLinks(driver, pattern, hikesDict);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        
    }
}
