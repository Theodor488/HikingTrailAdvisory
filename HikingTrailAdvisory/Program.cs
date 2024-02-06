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
                    ScrapeHikeLinks(driver, nextUrl, pattern, hikesDict);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Dictionary<string, string> ScrapeHikeLinks(IWebDriver driver, IWebElement url, string pattern, Dictionary<string, string> hikesDict)
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

        private static void ScrapeIndividualHikePage(IWebDriver driver, string hikeUrl, string pattern, Dictionary<string, string> hikesDict) 
        {
            
            //string headingText = headingElement.Text;

            driver.Navigate().GoToUrl(hikeUrl);

            string hikeName = driver.FindElement(By.ClassName("documentFirstHeading")).Text;
            var length = driver.FindElement(By.XPath("//dt[contains(., 'Length')]/following-sibling::dd")).Text;
            var elevationGain = driver.FindElement(By.XPath("//dt[contains(., 'Elevation Gain')]/following-sibling::dd")).Text;
            var highestPoint = driver.FindElement(By.XPath("//dt[contains(., 'Highest Point')]/following-sibling::dd")).Text;

        }
    }
}
