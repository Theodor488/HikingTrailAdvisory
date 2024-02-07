using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HikingTrailAdvisory
{
    internal class HikeInfoScraper
    {
        internal static Dictionary<string, string> ScrapeHikeLinks(IWebDriver driver, IWebElement url, string pattern, Dictionary<string, string> hikesDict)
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
                    //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", link);

                    hikesDict.Add(match.Value, link);
                    Console.WriteLine(match.Value + " " + link);

                    ScrapeIndividualHikePage(driver, link, pattern, hikesDict);
                }
            }

            // Navigate to the page containing the hikes
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", url);
            url.Click();

            return hikesDict;
        }

        internal static void ScrapeIndividualHikePage(IWebDriver driver, string hikeUrl, string pattern, Dictionary<string, string> hikesDict)
        {
            driver.Navigate().GoToUrl(hikeUrl);

            // Gets hike fields
            string hikeName = driver.FindElement(By.ClassName("documentFirstHeading")).Text;
            var length = driver.FindElement(By.XPath("//dt[contains(., 'Length')]/following-sibling::dd")).Text;
            var elevationGain = driver.FindElement(By.XPath("//dt[contains(., 'Elevation Gain')]/following-sibling::dd")).Text;
            var highestPoint = driver.FindElement(By.XPath("//dt[contains(., 'Highest Point')]/following-sibling::dd")).Text;
            var coords = driver.FindElement(By.XPath("//img[contains(@src, 'location.svg')]/following-sibling::span//span")).Text;
            var difficulty = driver.FindElement(By.XPath("//dt[contains(text(), 'Calculated Difficulty')]/following-sibling::dd")).Text;
            var description = driver.FindElement(By.Id("hike-body-text")).Text;
            string link = hikeUrl;

            // Hike Object


        }
    }
}
