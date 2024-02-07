using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
            string pattern = @"[^/]+$";
            Dictionary<string, string> hikesDict = new Dictionary<string, string>();

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
                    IWebElement nextUrl = driver.FindElement(By.CssSelector("li.next a"));
                    HikeInfoScraper.ScrapeHikeLinks(driver, nextUrl, pattern, hikesDict);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        
    }
}
