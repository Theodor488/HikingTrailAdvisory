using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;
using System.Runtime.InteropServices;
using OpenQA.Selenium.Chrome;

namespace HikingTrailAdvisory
{
    internal class HikeInfoScraper
    {
        internal static Dictionary<string, Hike> ScrapeHikeLinks(IWebDriver driver, IWebElement url, string pattern, Dictionary<string, Hike> hikesDict)
        {
            // Find all <a> elements
            var hikes = driver.FindElements(By.TagName("a"));

            // total count
            string totalHikes = GetElementTextOrDefault(driver, "XPath", "//div[@class='search-result-header']//span[@class='search-count']/span");
            int.TryParse(totalHikes, out int totalHikesCount);
            Console.WriteLine($"Total Hikes Count: {totalHikesCount}");

            int pageIdx = 0;
            List<string> hikeLinksList = new List<string>();

            // Last Page Index
            string lastPageIdxString = driver.FindElement(By.CssSelector("li.last a")).Text;
            int.TryParse(lastPageIdxString, out int lastPageIdx);

            // Get all links. Loop through all pages.
            while (lastPageIdx > pageIdx)
            {
                // Active Page Idx
                string activePageIdxString = driver.FindElement(By.CssSelector("li.active > span")).Text;
                int.TryParse(activePageIdxString, out pageIdx);

                // Next Page Link
                var nextPageUrl = driver.FindElement(By.XPath("//li[@class='active']/following-sibling::li/a")).GetAttribute("href");

                foreach (var hike in hikes) 
                {
                    string href = hike.GetAttribute("href");

                    if (!string.IsNullOrEmpty(href) && href.Contains("/go-hiking/hikes/"))
                    {
                        if (!hikeLinksList.Contains(href))
                        {
                            hikeLinksList.Add(href);
                        }
                    }
                }

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));

                wait.until(ExpectedConditions.visiblityofElementToBeClickable(By.locator(“”)));

                driver = new ChromeDriver();
                driver.Navigate().GoToUrl(nextPageUrl);
            }

            // Create Name : link Dictionary of hike names and links
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

            // Navigate to the page containing the hikes
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", url);
            url.Click();

            // Serialize to json
            var json = JsonConvert.SerializeObject(hikesDict); // Newtonsoft

            return hikesDict;
        }

        internal static Hike ScrapeIndividualHikePage(IWebDriver driver, string hikeUrl, string pattern, Dictionary<string, Hike> hikesDict, Hike hike)
        {
            driver.Navigate().GoToUrl(hikeUrl);

            // Gets hike fields

            string hikeName = GetElementTextOrDefault(driver, "ClassName", "documentFirstHeading");
            string length = GetElementTextOrDefault(driver, "XPath", "//dt[contains(., 'Length')]/following-sibling::dd");
            string elevationGain = GetElementTextOrDefault(driver, "XPath", "//dt[contains(., 'Elevation Gain')]/following-sibling::dd");
            string highestPoint = GetElementTextOrDefault(driver, "XPath", "//dt[contains(., 'Highest Point')]/following-sibling::dd");
            string coords = GetElementTextOrDefault(driver, "XPath", "//img[contains(@src, 'location.svg')]/following-sibling::span//span");
            string difficulty = GetElementTextOrDefault(driver, "XPath", "//dt[contains(text(), 'Calculated Difficulty')]/following-sibling::dd");
            string description = GetElementTextOrDefault(driver, "Id", "hike-body-text");
            string link = hikeUrl;

            /*
            string hikeName = driver.FindElement(By.ClassName("documentFirstHeading")).Text;
            var length = driver.FindElement(By.XPath("//dt[contains(., 'Length')]/following-sibling::dd")).Text;
            var elevationGain = driver.FindElement(By.XPath("//dt[contains(., 'Elevation Gain')]/following-sibling::dd")).Text;
            var highestPoint = driver.FindElement(By.XPath("//dt[contains(., 'Highest Point')]/following-sibling::dd")).Text;
            var coords = driver.FindElement(By.XPath("//img[contains(@src, 'location.svg')]/following-sibling::span//span")).Text;
            var difficulty = driver.FindElement(By.XPath("//dt[contains(text(), 'Calculated Difficulty')]/following-sibling::dd")).Text;
            var description = driver.FindElement(By.Id("hike-body-text")).Text;

            string link = hikeUrl;*/

            // Hike Object
            hike.Name = hikeName;
            hike.Length = length;
            hike.ElevationGain = elevationGain;
            hike.HighestPoint = highestPoint;
            hike.Coords = coords;
            hike.Difficulty = difficulty;
            hike.Description = description;
            hike.Link = link;

            return hike;
        }

        private static string GetElementTextOrDefault(IWebDriver driver, string pathType, string path, string defaultValue="")
        {
            try
            {
                if (pathType == "XPath")
                {
                    return driver.FindElement(By.XPath(path)).Text;
                }
                else if (pathType == "Id")
                {
                    return driver.FindElement(By.Id(path)).Text;
                }
                else if (pathType == "ClassName")
                {
                    return driver.FindElement(By.ClassName(path)).Text;
                }
                else
                {
                    return $"Error: {pathType} is an unkown pathType";
                }
                
            }
            catch (NoSuchElementException)
            {
                return defaultValue;
            }
            catch (StaleElementReferenceException)
            {
                // Reattempt to find the element after waiting for it to become stable
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                return defaultValue;
            }

        }
    }
}
