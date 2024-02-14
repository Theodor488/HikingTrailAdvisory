using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikingTrailAdvisory
{
    public static class ScraperHelper
    {
        public static string GetElementTextOrDefault(IWebDriver driver, string pathType, string path, string defaultValue = "")
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
