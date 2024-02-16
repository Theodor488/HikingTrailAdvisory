using OpenQA.Selenium;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace HikingTrailAdvisory
{ 
    internal class HikeInfoScraper
    {
        internal static Dictionary<string, Hike> ScrapeHikeLinks(IWebDriver driver, string pattern, Dictionary<string, Hike> hikesDict)
        {
            List<string> hikeLinksList = new List<string>();

            // Initialize current page index
            int pageIdx = 0;

            // Last page index
            string lastPageIdxString = driver.FindElement(By.CssSelector("li.last a")).Text;
            int.TryParse(lastPageIdxString, out int lastPageIdx);

            // Total number of hikes
            string totalHikes = ScraperHelper.GetElementTextOrDefault(driver, "XPath", "//div[@class='search-result-header']//span[@class='search-count']/span");
            int.TryParse(totalHikes, out int totalHikesCount);
            Console.WriteLine($"Total Hikes Count: {totalHikesCount}");

            // Get all links. Loop through all pages.
            while (lastPageIdx >= pageIdx)
            {
                Console.WriteLine($"PageIdx: {pageIdx} / {lastPageIdx}");
                Console.WriteLine($"Hike Count: {hikeLinksList.Count} / {totalHikes}");
                pageIdx = ProcessHikesAndNavigateToNextPage(driver, hikeLinksList);
            }

            PopulateHikesDictionary(driver, pattern, hikesDict, hikeLinksList);
            var json = JsonConvert.SerializeObject(hikesDict);

            return hikesDict;
        }

        private static int ProcessHikesAndNavigateToNextPage(IWebDriver driver, List<string> hikeLinksList)
        {
            // Current Page Idx
            string currentPageIdxString = driver.FindElement(By.CssSelector("li.active > span")).Text;
            int.TryParse(currentPageIdxString, out int pageIdx);

            // Fetch list of hike elements for the current page
            var hikes = driver.FindElements(By.XPath("//a[contains(@href, 'go-hiking/hikes/')]"));
            var nextPageLink = new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(ExpectedConditions.ElementToBeClickable(By.XPath("//li[@class='active']/following-sibling::li/a")));

            PopulateListOfHikes(hikeLinksList, hikes);

            // Scroll into view and click the next page link
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", nextPageLink);
            Thread.Sleep(500);
            nextPageLink.Click();

            // Wait for the next page to fully load before continuing the loop
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.FindElement(By.CssSelector("li.active > span")).Displayed);
            return pageIdx;
        }

        private static void PopulateHikesDictionary(IWebDriver driver, string pattern, Dictionary<string, Hike> hikesDict, List<string> hikeLinksList)
        {
            foreach (string link in hikeLinksList)
            {
                Match match = Regex.Match(link, pattern);

                if (match.Success)
                {
                    string hikeName = match.Value;
                    Hike hike = new Hike();

                    hike = ScrapeIndividualHikePage(driver, link, pattern, hikesDict, hike);

                    hikesDict.Add(hikeName, hike);
                }
            }
        }

        internal static void PopulateListOfHikes(List<string> hikeLinksList, System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> hikes)
        {
            foreach (var hike in hikes)
            {
                string href = hike.GetAttribute("href");

                if (!string.IsNullOrEmpty(href) && href.Contains("/go-hiking/hikes/") && !hikeLinksList.Contains(href))
                {
                    hikeLinksList.Add(href);
                }
            }
        }

        internal static Hike ScrapeIndividualHikePage(IWebDriver driver, string hikeUrl, string pattern, Dictionary<string, Hike> hikesDict, Hike hike)
        {
            driver.Navigate().GoToUrl(hikeUrl);

            // Populate Hike Object
            hike.Name = ScraperHelper.GetElementTextOrDefault(driver, "ClassName", "documentFirstHeading"); ;
            hike.Length = ScraperHelper.GetElementTextOrDefault(driver, "XPath", "//dt[contains(., 'Length')]/following-sibling::dd"); ;
            hike.ElevationGain = ScraperHelper.GetElementTextOrDefault(driver, "XPath", "//dt[contains(., 'Elevation Gain')]/following-sibling::dd"); ;
            hike.HighestPoint = ScraperHelper.GetElementTextOrDefault(driver, "XPath", "//dt[contains(., 'Highest Point')]/following-sibling::dd"); ;
            hike.Coords = ScraperHelper.GetElementTextOrDefault(driver, "XPath", "//img[contains(@src, 'location.svg')]/following-sibling::span//span"); ;
            hike.Difficulty = ScraperHelper.GetElementTextOrDefault(driver, "XPath", "//dt[contains(text(), 'Calculated Difficulty')]/following-sibling::dd"); ;
            hike.Description = ScraperHelper.GetElementTextOrDefault(driver, "Id", "hike-body-text"); ;
            hike.Link = hikeUrl;

            return hike;
        }
    }
}
