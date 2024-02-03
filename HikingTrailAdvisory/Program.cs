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
            var url = "https://www.wta.org/go-outside/hikes";
            var client = new HttpClient();
            var html = await client.GetStringAsync(url);
            string pattern = @"[^/]+$";

            new DriverManager().SetUpDriver(new ChromeConfig());
            IWebDriver driver = new ChromeDriver();

            List<string> hikeLinks = ScrapeHikeLinks(driver);
            //List<string> hikeNames = ScrapeHikeNames(html);

            foreach (string link in hikeLinks)
            {
                Match match = Regex.Match(link, pattern);

                if (match.Success)
                {
                    Console.WriteLine(match.Value);
                }
            }
        }

        private static List<string> ScrapeHikeLinks(IWebDriver driver)
        {
            // Navigate to the page containing the hikes
            driver.Navigate().GoToUrl("https://www.wta.org/go-outside/hikes");

            // Find all <a> elements
            var links = driver.FindElements(By.TagName("a"));

            // Filter to get only the links that lead to hike details
            List<string> hikeLinks = links.Where(link => link.GetAttribute("href").Contains("/go-hiking/hikes/")).Select(link => link.GetAttribute("href")).Distinct().ToList();

            return hikeLinks;

            /*foreach (var link in hikeLinks)
            {
                Console.WriteLine(link);
            }*/
        }

        static List<string> ScrapeHikeNames(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            List<string> HikeNames = new List<string>();

            var hikeNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='listing-image backpack-wrapper backpack-hikebuttons']");

            if (hikeNodes != null)
            {
                foreach (var hikeNode in hikeNodes)
                {
                    var hikeName = hikeNode.GetAttributeValue("data-hikename", string.Empty);
                    Console.WriteLine(hikeName);
                    HikeNames.Add(hikeName);
                }
            }
            else
            {
                Console.WriteLine("No hikes found");
            }

            return HikeNames;
        }
    }
}
