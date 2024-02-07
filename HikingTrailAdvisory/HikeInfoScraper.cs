using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Support.UI;

namespace HikingTrailAdvisory
{
    internal class HikeInfoScraper
    {
        internal static Dictionary<string, Hike> ScrapeHikeLinks(IWebDriver driver, IWebElement url, string pattern, Dictionary<string, Hike> hikesDict)
        {
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
                    //hikesDict.Add(match.Value, link);
                    //Console.WriteLine(match.Value + " " + link);
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
