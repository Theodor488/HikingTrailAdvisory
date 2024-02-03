using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System;
using System.Text.RegularExpressions;

namespace HikingTrailAdvisory
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string url = "https://www.wta.org/go-outside/hikes";
            string pattern = @"[^/]+$";

            new DriverManager().SetUpDriver(new ChromeConfig());
            IWebDriver driver = new ChromeDriver();

            Dictionary<string, string> hikesDict = ScrapeHikeLinks(driver, url, pattern);
        }

        private static Dictionary<string, string> ScrapeHikeLinks(IWebDriver driver, string url, string pattern)
        {
            Dictionary<string, string> hikesDict = new Dictionary<string, string>();

            // Navigate to the page containing the hikes
            driver.Navigate().GoToUrl(url);

            // Find all <a> elements
            var hikes = driver.FindElements(By.TagName("a"));

            // Filter to get only the links that lead to hike details
            List<string> hikeLinks = hikes.Where(hike => hike.GetAttribute("href").Contains("/go-hiking/hikes/")).Select(link => link.GetAttribute("href")).Distinct().ToList();

            // Create Name : link Dictionary of hike names and links
            foreach (string link in hikeLinks)
            {
                Match match = Regex.Match(link, pattern);

                if (match.Success)
                {
                    hikesDict.Add(match.Value, link);
                    Console.WriteLine(match.Value + " " + link);
                }
            }

            return hikesDict;
        }
    }
}
